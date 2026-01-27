using FashionStore.Repository; // Thay bằng namespace của bạn
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Controllers
{
    public class CufflinkController : Controller
    {
        private readonly fashionDbContext _context;

        public CufflinkController(fashionDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var cufflinksList = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == 1)
                .ToListAsync();

            return View(cufflinksList);
        }
    }
}