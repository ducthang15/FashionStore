using FashionStore.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Controllers
{
    public class TuxedoController : BaseController
    {

        public TuxedoController(fashionDbContext context) : base(context)
        {
        }

        public async Task<IActionResult> Index()
        {
            var ShoeList = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == 10)
                .ToListAsync();
            ViewBag.CurrentCategoryId = 10;
            return View(ShoeList);
        }
    }
}