using FashionStore.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Controllers
{
    public class SuitController : Controller
    {
        private readonly fashionDbContext _context;

        public SuitController(fashionDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var ShoeList = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == 8)
                .ToListAsync();

            return View(ShoeList);
        }
    }
}