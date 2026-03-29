using FashionStore.Repository;
using FashionStore.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Controllers
{
    public class BlogController : Controller
    {
        private readonly fashionDbContext _context;

        public BlogController(fashionDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? id, int page = 1)
        {
            ViewBag.Categories = await _context.NewsCategories.ToListAsync();
            var query = _context.News
                .Include(n => n.NewsCategory)
                .Where(n => n.IsPublished == true);
            if (id.HasValue)
            {
                query = query.Where(n => n.NewsCategoryId == id);
            }
            int pageSize = 10;
            int totalItems = await query.CountAsync();
            var newsList = await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)  
                .Take(pageSize)
                .ToListAsync();
            var featuredNews = await _context.News
                .Where(n => n.IsPublished)
                .OrderByDescending(n => n.CreatedAt)
                .Take(5)
                .ToListAsync();

            ViewBag.FeaturedNews = featuredNews;
            ViewBag.CurrentCategoryId = id;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            return View(newsList);
        }

        public async Task<IActionResult> Details(string slug)
        {
            var news = await _context.News
                .Include(n => n.NewsCategory)
                .FirstOrDefaultAsync(n => n.Slug == slug);

            if (news == null) return NotFound();
            var suitProducts = await _context.Products
               .Where(p => p.CategoryId == 8)
               .Take(5)
               .ToListAsync();

            ViewBag.SuitProducts = suitProducts;
            return View(news);
        }
        public async Task<IActionResult> ShuttleService()
        {
            // Giả sử ID của Shuttle Service trong SQL là 5
            int serviceCategoryId = 4;

            var posts = await _context.News
                .Where(n => n.NewsCategoryId == serviceCategoryId && n.IsPublished)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
            var category = await _context.NewsCategories.FindAsync(serviceCategoryId);
            ViewBag.CategoryName = category?.CategoryName;
            return View("ShuttleService", posts);
        }
        public async Task<IActionResult> OnlineTailoring()
        {
            int serviceCategoryId = 2;

            var posts = await _context.News
                .Where(n => n.NewsCategoryId == serviceCategoryId && n.IsPublished)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
            var category = await _context.NewsCategories.FindAsync(serviceCategoryId);
            ViewBag.CategoryName = category?.CategoryName;
            return View("OnlineTailoring", posts);
        }
        public async Task<IActionResult> TailoringAtHome()
        {
            int serviceCategoryId = 1;

            var posts = await _context.News
                .Where(n => n.NewsCategoryId == serviceCategoryId && n.IsPublished)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
            var category = await _context.NewsCategories.FindAsync(serviceCategoryId);
            ViewBag.CategoryName = category?.CategoryName;
            return View("TailoringAtHome", posts);
        }
        public async Task<IActionResult> TailoringProcess()
        {
            int serviceCategoryId = 5;

            var posts = await _context.News
                .Where(n => n.NewsCategoryId == serviceCategoryId && n.IsPublished)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
            var category = await _context.NewsCategories.FindAsync(serviceCategoryId);
            ViewBag.CategoryName = category?.CategoryName;
            return View("TailoringProcess", posts);
        }
        public async Task<IActionResult> TailoringUniforms()
        {
            int serviceCategoryId = 3;

            var posts = await _context.News
                .Where(n => n.NewsCategoryId == serviceCategoryId && n.IsPublished)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
            var category = await _context.NewsCategories.FindAsync(serviceCategoryId);
            ViewBag.CategoryName = category?.CategoryName;
            return View("TailoringUniforms", posts);
        }
    }
}