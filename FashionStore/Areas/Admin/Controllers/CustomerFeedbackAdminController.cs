using FashionStore.Repository;
using FashionStore.Repository.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Areas.Admin.Controllers
{
    public class CustomerFeedbackAdminController : BaseAdminController
    {
        private readonly fashionDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CustomerFeedbackAdminController(fashionDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.CustomerFeedbacks.Include(f => f.FeedbackImages)
        .OrderByDescending(f => f.CreatedAt).ToListAsync());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CustomerFeedback feedback, List<IFormFile> files)
        {
            ModelState.Remove("FeedbackImages");

            if (files != null && files.Count > 0)
            {
                feedback.CreatedAt = DateTime.Now;
                _context.Add(feedback);
                await _context.SaveChangesAsync();
                foreach (var file in files)
                {
                    var path = await UploadFile(file);
                    var img = new FeedbackImage
                    {
                        ImageUrl = path,
                        FeedbackId = feedback.FeedbackId 
                    };
                    _context.FeedbackImages.Add(img);
                }
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(feedback);
        }
        public async Task<IActionResult> Edit(int id)
        {
            var feedback = await _context.CustomerFeedbacks
                .Include(f => f.FeedbackImages)
                .FirstOrDefaultAsync(f => f.FeedbackId == id);

            if (feedback == null) return NotFound();

            return View(feedback);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(CustomerFeedback feedback, List<IFormFile> files, List<int> deleteImageIds)
        {
            var existing = await _context.CustomerFeedbacks
                .Include(f => f.FeedbackImages)
                .FirstOrDefaultAsync(f => f.FeedbackId == feedback.FeedbackId);

            if (existing == null) return NotFound();
            existing.Content = feedback.Content;
            existing.Tags = feedback.Tags;
            existing.CustomerName = feedback.CustomerName;

            if (deleteImageIds != null && deleteImageIds.Count > 0)
            {
                var imagesToDelete = existing.FeedbackImages
                    .Where(i => deleteImageIds.Contains(i.ImageId))
                    .ToList();

                foreach (var img in imagesToDelete)
                {
                    var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, img.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }

                    _context.FeedbackImages.Remove(img);
                }
            }
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    var path = await UploadFile(file);
                    _context.FeedbackImages.Add(new FeedbackImage
                    {
                        ImageUrl = path,
                        FeedbackId = existing.FeedbackId
                    });
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.CustomerFeedbacks.FindAsync(id);
            if (item != null)
            {
                _context.CustomerFeedbacks.Remove(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        private async Task<string> UploadFile(IFormFile file)
        {
            string fileName = Guid.NewGuid().ToString() + "-" + file.FileName;
            string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "images", "feedbacks");
            if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);

            string filePath = Path.Combine(uploadDir, fileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return "/images/feedbacks/" + fileName;
        }
    }
}