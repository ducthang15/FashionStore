using FashionStore.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Controllers
{
    public class GalleryController : Controller
    {
        private readonly fashionDbContext _context;

        public GalleryController(fashionDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var gallery = await _context.CustomerFeedbacks
                .Include(f => f.FeedbackImages)
                .Where(x => x.IsPublished ==true)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
            return View(gallery);
        }
    }
}