using FashionStore.Models;
using FashionStore.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using FashionStore.Utilities;

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
            // product show
            var products = await _context.Products
            .Include(p => p.Category)
            .Where(p => p.Category.CategoryName == "Suit")
            .ToListAsync();
            //review customer
            ViewBag.Reviews = await _context.CustomerReviews
            .Where(x => x.IsPublished)
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync();
            return View(products);
        }
        [Route("product/{slug}")]
        public async Task<IActionResult> Details(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(m => m.Slug == slug);
            if (product == null)
            {
                return NotFound();
            }
            var relatedProducts = await _context.Products.Include(p => p.Category)
             .Where(p =>
             p.CategoryId == product.CategoryId &&
             p.ProductId != product.ProductId)
                .OrderByDescending(p => p.ProductId)
                .Take(8)
                .ToListAsync();
            ViewBag.RelatedProducts = relatedProducts;
            return View(product);
        }
        public IActionResult Privacy()
        {
            return View();
        }
        [Route("return-policy")]
        public IActionResult ReturnPolicy()
        {
            return View();
        }
        [Route("all-product")]
        public async Task<IActionResult> Allproduct(string? sort, int page = 1)
        {
            int pageSize = 16;

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

            int totalItems = await query.CountAsync();

            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.CurrentSort = sort;

            return View(products);
        }
        [Route("care")]
        public IActionResult Care()
        {
            return View();
        }
        [Route("FAQs")]
        public IActionResult FAQs()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new Models.ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}