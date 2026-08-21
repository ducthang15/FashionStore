using FashionStore.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Controllers
{
    public class ShoeController : BaseController
    {
        public ShoeController(fashionDbContext context) : base(context)
        {
        }

        public async Task<IActionResult> Index(string? sort, int page = 1)
        {
            if (page < 1)
            {
                return NotFound();
            }

            int pageSize = 8;

            var query = _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == 3);

            if (sort == "name")
            {
                query = query.OrderBy(p => p.ProductName);
            }
            else
            {
                query = query.OrderByDescending(p => p.CreatedAt ?? DateTime.MinValue);
            }

            int totalItems = await query.CountAsync();

            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            if (totalPages == 0 && page > 1)
            {
                return NotFound();
            }

            if (totalPages > 0 && page > totalPages)
            {
                return NotFound();
            }

            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentSort = sort;
            ViewBag.CurrentCategoryId = 3;

            return View(products);
        }
    }
}