using FashionStore.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Controllers
{
    public class FabricController : BaseController
    {
        public FabricController(fashionDbContext context) : base(context)
        {
        }

        public async Task<IActionResult> Index()
        {
            var ShoeList = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == 7)
                .ToListAsync();
            ViewBag.CurrentCategoryId = 7;
            return View(ShoeList);
        }
    }
}