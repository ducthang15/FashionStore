using FashionStore.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Controllers
{
    public class ShirtController : BaseController
    {
        public ShirtController(fashionDbContext context) : base(context)
        {
        }

        public async Task<IActionResult> Index()
        {
            var ShoeList = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == 11)
                .ToListAsync();
            ViewBag.CurrentCategoryId = 11;
            return View(ShoeList);
        }
    }
}