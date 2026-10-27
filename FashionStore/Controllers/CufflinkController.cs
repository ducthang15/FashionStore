using FashionStore.Repository; // Thay bằng namespace của bạn
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Controllers
{
    public class CufflinkController : BaseController
    {
        public CufflinkController(fashionDbContext context) : base(context)
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
                .Where(p => p.CategoryId == 1);
            if (sort == "name")
            {
                query = query
                    .OrderBy(p => p.ProductName)
                    .ThenBy(p => p.ProductId);
            }
            else
            {
                query = query
                    .OrderBy(p => p.DisplayOrder == null ? 1 : 0)
                    .ThenBy(p => p.DisplayOrder)
                    .ThenByDescending(p => p.CreatedAt)
                    .ThenByDescending(p => p.ProductId);
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
            ViewBag.CurrentCategoryId = 1;

            return View(products);
        }
    }
}