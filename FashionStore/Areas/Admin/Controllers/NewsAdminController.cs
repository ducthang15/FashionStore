using FashionStore.Repository;
using FashionStore.Repository.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class NewsAdminController : Controller
    {
        private readonly fashionDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public NewsAdminController(fashionDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.News.OrderByDescending(n => n.CreatedAt).ToListAsync());
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(News news, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                // Upload ảnh bìa
                if (file != null)
                {
                    news.ImageUrl = await UploadFile(file);
                }
                else
                {
                    news.ImageUrl = "/images/no-image.png";
                }

                news.CreatedAt = DateTime.Now;
                _context.Add(news);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(news);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var news = await _context.News.FindAsync(id);
            if (news != null)
            {
                _context.News.Remove(news);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        private async Task<string> UploadFile(IFormFile file)
        {
            string fileName = Guid.NewGuid().ToString() + "-" + file.FileName;
            string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "images", "news");
            string filePath = Path.Combine(uploadDir, fileName);

            if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return "/images/news/" + fileName;
        }
    }
}