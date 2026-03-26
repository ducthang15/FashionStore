using FashionStore.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Controllers
{
    public class PantsController : BaseController
    {
        public PantsController(fashionDbContext context) : base(context)
        {
        }

        public async Task<IActionResult> Index()
        {
            var ShoeList = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == 9)
                .ToListAsync();
            ViewBag.CurrentCategoryId = 9;
            return View(ShoeList);
        }
    }
}