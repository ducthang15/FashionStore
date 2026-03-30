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
            int pageSize = 8;

            var query = _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == 1);
            if (sort == "name")
            {
                query = query.OrderBy(p => p.ProductName);
            }
            else
            {
                query = query.OrderByDescending(p => p.CreatedAt ?? DateTime.MinValue);
            }

            int totalItems = await query.CountAsync();

            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.CurrentSort = sort;
            ViewBag.CurrentCategoryId = 1;

            return View(products);
        }
    }
}