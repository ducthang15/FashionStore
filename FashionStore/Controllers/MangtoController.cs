using FashionStore.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Controllers
{
    public class MangtoController : BaseController
    {
        public MangtoController(fashionDbContext context) : base(context)
        {
        }

        public async Task<IActionResult> Index()
        {
            var ShoeList = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == 13)
                .ToListAsync();
            ViewBag.CurrentCategoryId = 13;
            return View(ShoeList);
        }
    }
}