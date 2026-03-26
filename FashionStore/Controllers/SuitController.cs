using FashionStore.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Controllers
{
    public class SuitController : BaseController
    {
        public SuitController(fashionDbContext context) : base(context)
        {
        }

        public async Task<IActionResult> Index()
        {
            var ShoeList = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == 8)
                .ToListAsync();
            ViewBag.CurrentCategoryId = 8;
            return View(ShoeList);
        }
    }
}