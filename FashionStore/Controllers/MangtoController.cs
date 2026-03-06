using FashionStore.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Controllers
{
    public class MangtoController : Controller
    {
        private readonly fashionDbContext _context;

        public MangtoController(fashionDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var ShoeList = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == 13)
                .ToListAsync();

            return View(ShoeList);
        }
    }
}