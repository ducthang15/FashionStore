using FashionStore.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Controllers
{
    public class CoatController : BaseController
    {
        public CoatController(fashionDbContext context) : base(context)
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
                .Where(p => p.CategoryId == 13);

            if (sort == "name")
            {
                query = query.OrderBy(p => p.ProductName);
            }
            else
            {
                query = query.OrderByDescending(
                    p => p.CreatedAt ?? DateTime.MinValue
                );
            }

            int totalItems = await query.CountAsync();

            int totalPages = (int)Math.Ceiling(
                (double)totalItems / pageSize
            );

            // Nếu có sản phẩm nhưng page vượt quá số trang thì 404.
            // Nếu chưa có sản phẩm thì vẫn cho phép mở trang Coat.
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
            ViewBag.CurrentCategoryId = 13;

            return View(products);
        }
    }
}