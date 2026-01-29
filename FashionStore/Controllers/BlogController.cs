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

        public async Task<IActionResult> Index()
        {
            var newsList = await _context.News
                .Where(n => n.IsPublished == true) // Chỉ lấy bài đã Public
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
            return View(newsList);
        }
        public async Task<IActionResult> Details(int id)
        {
            var news = await _context.News.FindAsync(id);
            if (news == null) return NotFound();

            return View(news);
        }
    }
}