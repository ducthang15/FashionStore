using Microsoft.AspNetCore.Mvc;
using FashionStore.Models; // G?i ??n th? m?c Models
using Microsoft.EntityFrameworkCore;
using FashionStore.Repository;

namespace FashionStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly fashionDbContext _context;

        public HomeController(fashionDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // L?y toàn b? s?n ph?m t? SQL Server
            var products = await _context.Products.ToListAsync();
            return View(products);
        }
    }
}