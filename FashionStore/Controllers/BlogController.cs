using FashionStore.Repository;
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
            if (page < 1)
            {
                return RedirectToAction("Index", new { id, page = 1 });
            }

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

            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            if (page > totalPages && totalPages > 0)
            {
                return RedirectToAction("Index", new { id, page = totalPages });
            }

            var newsList = await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var featuredNews = await _context.News
                .Where(n => n.IsPublished)
                .OrderBy(n => n.CreatedAt)
                .Take(5)
                .ToListAsync();

            ViewBag.FeaturedNews = featuredNews;
            ViewBag.CurrentCategoryId = id;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(newsList);
        }
        public async Task<IActionResult> Details(string slug)
        {
            var news = await _context.News
                .Include(n => n.NewsCategory)
                .FirstOrDefaultAsync(n => n.Slug == slug);

            if (news == null) return NotFound();
            string cookieKey = "Viewed_News_" + news.NewsId; // Giả sử khóa chính của bạn là NewsId
            if (!Request.Cookies.ContainsKey(cookieKey))
            {
                news.ViewCount++;
                await _context.SaveChangesAsync();

                // Tạo Cookie để đánh dấu khách đã xem bài này
                CookieOptions option = new CookieOptions
                {
                    Expires = DateTime.Now.AddMinutes(2),
                    HttpOnly = true,
                    IsEssential = true
                };
                Response.Cookies.Append(cookieKey, "v", option);
            }
            var suitProducts = await _context.Products
               .Where(p => p.CategoryId == 8)
               .Take(9)
               .ToListAsync();

            ViewBag.SuitProducts = suitProducts;
            var related = await _context.News
            .Where(n => n.NewsCategoryId == news.NewsCategoryId
                 && n.Slug != slug
                 && n.IsPublished)
            .OrderByDescending(n => n.CreatedAt)
                .Take(9)
                .ToListAsync();

            ViewBag.Related = related;
            return View(news);
        }
        [Route("blog/shuttle-service")]
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
        [Route("blog/online-tailoring")]
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
        [Route("blog/tailoring-at-home")]
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
        [Route("blog/tailoring-process")]
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
        [Route("blog/tailoring-uniforms")]
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