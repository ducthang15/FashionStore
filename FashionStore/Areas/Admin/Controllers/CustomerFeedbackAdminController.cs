using FashionStore.Repository;
using FashionStore.Repository.Models;
using Microsoft.AspNetCore.Mvc;
using FashionStore.Services;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Areas.Admin.Controllers
{
    public class CustomerFeedbackAdminController : BaseAdminController
    {
        private readonly fashionDbContext _context;
        private readonly IStorageService _storageService;

        public CustomerFeedbackAdminController(fashionDbContext context, IStorageService storageService)
        {
            _context = context;
            _storageService = storageService;
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
        public async Task<IActionResult> Create(CustomerFeedback feedback, List<IFormFile>? files)
        {
            ModelState.Remove("FeedbackImages");

            if (files != null && files.Count > 0)
            {
                feedback.CreatedAt = DateTime.Now;
                _context.Add(feedback);
                await _context.SaveChangesAsync();

                foreach (var file in files)
                {
                    var path = await _storageService.UploadFileAsync(file, "images/feedbacks");
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
        public async Task<IActionResult> Edit(CustomerFeedback feedback, List<IFormFile>? files, List<int>? deleteImageIds)
        {
            var existing = await _context.CustomerFeedbacks
                .Include(f => f.FeedbackImages)
                .FirstOrDefaultAsync(f => f.FeedbackId == feedback.FeedbackId);

            if (existing == null) return NotFound();

            existing.Content = feedback.Content;
            existing.Tags = feedback.Tags;
            existing.CustomerName = feedback.CustomerName;

            // Xóa các ảnh được chọn xóa
            if (deleteImageIds != null && deleteImageIds.Count > 0)
            {
                var imagesToDelete = existing.FeedbackImages
                    .Where(i => deleteImageIds.Contains(i.ImageId))
                    .ToList();

                foreach (var img in imagesToDelete)
                {
                    // Xóa ảnh trên Cloudinary nếu có cài đặt hàm DeleteFileAsync
                    if (!string.IsNullOrEmpty(img.ImageUrl))
                    {
                        await _storageService.DeleteFileAsync(img.ImageUrl);
                    }

                    _context.FeedbackImages.Remove(img);
                }
            }

            // Thêm các ảnh mới được tải lên
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    var path = await _storageService.UploadFileAsync(file, "images/feedbacks");
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
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.CustomerFeedbacks
                .Include(f => f.FeedbackImages)
                .FirstOrDefaultAsync(f => f.FeedbackId == id);

            if (item != null)
            {
                // Dọn sạch toàn bộ ảnh đính kèm trên Cloudinary trước khi xóa feedback
                if (item.FeedbackImages != null && item.FeedbackImages.Count > 0)
                {
                    foreach (var img in item.FeedbackImages)
                    {
                        if (!string.IsNullOrEmpty(img.ImageUrl))
                        {
                            await _storageService.DeleteFileAsync(img.ImageUrl);
                        }
                    }
                }

                _context.CustomerFeedbacks.Remove(item);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}