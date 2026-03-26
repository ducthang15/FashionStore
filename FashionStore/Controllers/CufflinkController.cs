using FashionStore.Repository; // Thay bằng namespace của bạn
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Controllers
{
    public class CufflinkController : BaseController
    {
        public CufflinkController(fashionDbContext context) : base(context)
        {
        }

        public async Task<IActionResult> Index()
        {
            var cufflinksList = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == 1)
                .ToListAsync();
            ViewBag.CurrentCategoryId = 1;
            return View(cufflinksList);
        }
    }
}