using FashionStore.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Controllers
{
    public class ShoeController : BaseController
    {
        public ShoeController(fashionDbContext context) : base(context)
        {
        }

        public async Task<IActionResult> Index()
        {
            var ShoeList = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == 3)
                .ToListAsync();
            ViewBag.CurrentCategoryId = 3;
            return View(ShoeList);
        }
    }
}