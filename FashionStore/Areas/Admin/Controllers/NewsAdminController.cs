using Microsoft.AspNetCore.Mvc.Rendering;
using FashionStore.Repository.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using FashionStore.Repository;
using FashionStore.Utilities;
using FashionStore.Services;

namespace FashionStore.Areas.Admin.Controllers
{
    public class NewsAdminController : BaseAdminController
    {
        private readonly fashionDbContext _context;
        private readonly IStorageService _storageService;

        public NewsAdminController(fashionDbContext context, IStorageService storageService)
        {
            _context = context;
            _storageService = storageService;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.News.OrderByDescending(n => n.CreatedAt).ToListAsync());
        }
        public IActionResult Create()
        {
            ViewBag.NewsCategoryId = new SelectList(_context.NewsCategories, "NewsCategoryId", "CategoryName");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(News news, IFormFile? file)
        {
            ModelState.Remove("NewsCategory");
            if (string.IsNullOrEmpty(news.Slug))
            {
                news.Slug = SlugHelper.GenerateSlug(news.Title);
            }
            if (_context.News.Any(n => n.Slug == news.Slug))
            {
                news.Slug += "-" + DateTime.Now.Ticks;
            }
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    news.ImageUrl = await UploadFile(file);
                }
                else
                {
                    news.ImageUrl = "/images/no-image.png";
                }

                news.CreatedAt = DateTime.Now;
                _context.Add(news);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.NewsCategoryId = new SelectList(_context.NewsCategories, "NewsCategoryId", "CategoryName", news.NewsCategoryId);
            return View(news);
        }
        public async Task<IActionResult> Edit(int id)
        {
            var news = await _context.News.FindAsync(id);
            if (news == null) return NotFound();

            ViewBag.NewsCategoryId = new SelectList(_context.NewsCategories, "NewsCategoryId", "CategoryName", news.NewsCategoryId);
            return View(news);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, News news, IFormFile? file)
        {
            if (id != news.NewsId) return NotFound();
            ModelState.Remove("NewsCategory");
            var existingNews = await _context.News.AsNoTracking().FirstOrDefaultAsync(n => n.NewsId == id);
            if (existingNews == null) return NotFound();
            if (string.IsNullOrEmpty(news.Slug))
            {
                news.Slug = SlugHelper.GenerateSlug(news.Title);
            }

            if (_context.News.Any(n => n.Slug == news.Slug && n.NewsId != id))
            {
                news.Slug += "-" + DateTime.Now.Ticks;
            }

            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    news.ImageUrl = await UploadFile(file);
                }
                else
                {
                    news.ImageUrl = existingNews.ImageUrl;
                }
                news.CreatedAt = existingNews.CreatedAt;
                var existing = await _context.News.FindAsync(id);
                if (existing == null) return NotFound();

                if (file != null)
                {
                    existing.ImageUrl = await UploadFile(file);
                }
                existing.Title = news.Title;
                existing.Content = news.Content;
                existing.Slug = news.Slug;
                existing.NewsCategoryId = news.NewsCategoryId;
                existing.IsPublished = news.IsPublished;
                existing.MetaDescription = news.MetaDescription;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.NewsCategoryId = new SelectList(_context.NewsCategories, "NewsCategoryId", "CategoryName", news.NewsCategoryId);
            return View(news);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var news = await _context.News.FindAsync(id);
            if (news != null)
            {
                _context.News.Remove(news);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        private async Task<string?> UploadFile(IFormFile file)
        {
            return await _storageService.UploadFileAsync(file, "images/news");
        }
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Json(new { error = "No file uploaded" });

            var url = await _storageService.UploadFileAsync(file, "images/news");
            return Json(new { location = url });
        }

    }
}