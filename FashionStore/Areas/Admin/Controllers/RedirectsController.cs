using Microsoft.EntityFrameworkCore;
using FashionStore.Repository.Models;
using Microsoft.AspNetCore.Mvc;
using FashionStore.Repository;

namespace FashionStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RedirectsController : Controller
    {
        private readonly fashionDbContext _context;

        public RedirectsController(fashionDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var list = await _context.Redirects.OrderByDescending(r => r.CreatedAt).ToListAsync();
            return View(list);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Redirect redirect)
        {
            if (ModelState.IsValid)
            {
                // Tự động làm sạch dữ liệu trước khi lưu: cắt khoảng trắng, bỏ dấu "/" ở đầu/cuối, chuyển về chữ thường
                redirect.OldUrl = redirect.OldUrl.Trim().Trim('/').ToLower();
                redirect.NewUrl = redirect.NewUrl.Trim().Trim('/');
                redirect.CreatedAt = System.DateTime.Now;

                // Kiểm tra trùng lặp OldUrl
                var isExist = await _context.Redirects.AnyAsync(r => r.OldUrl == redirect.OldUrl);
                if (isExist)
                {
                    ModelState.AddModelError("OldUrl", "Đường dẫn cũ này đã được cấu hình chuyển hướng trước đó rồi!");
                    return View(redirect);
                }

                _context.Add(redirect);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(redirect);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var redirect = await _context.Redirects.FindAsync(id);
            if (redirect == null) return NotFound();

            return View(redirect);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Redirect redirect)
        {
            if (id != redirect.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    redirect.OldUrl = redirect.OldUrl.Trim().Trim('/').ToLower();
                    redirect.NewUrl = redirect.NewUrl.Trim().Trim('/');

                    _context.Update(redirect);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Redirects.Any(e => e.Id == redirect.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(redirect);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var redirect = await _context.Redirects.FindAsync(id);
            if (redirect != null)
            {
                _context.Redirects.Remove(redirect);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}