using FashionStore.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Controllers
{
    public class ProductsController : BaseController
    {
        public ProductsController(fashionDbContext context) : base(context)
        {
        }

        public async Task<IActionResult> Allproduct(string? sort)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            if (sort == "name")
            {
                query = query.OrderBy(p => p.ProductName);
            }
            else
            {
                query = query.OrderByDescending(p => p.CreatedAt ?? DateTime.MinValue);
            }

            var products = await query.ToListAsync();

            return View(products);
        }
    }
}