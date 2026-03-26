using FashionStore.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Controllers
{
    public class BlazerController : BaseController
    {
        public BlazerController(fashionDbContext context) : base(context)
        {
        }

        public async Task<IActionResult> Index()
        {
            var ShoeList = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == 12)
                .ToListAsync();
            ViewBag.CurrentCategoryId = 12;
            return View(ShoeList);
        }
    }
}