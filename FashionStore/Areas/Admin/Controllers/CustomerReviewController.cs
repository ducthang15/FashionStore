using FashionStore.Repository;
using FashionStore.Repository.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using FashionStore.Services;

namespace FashionStore.Areas.Admin.Controllers
{
    public class CustomerReviewController : BaseAdminController
    {
        private readonly fashionDbContext _context;
        private readonly IStorageService _storageService;

        public CustomerReviewController(fashionDbContext context, IStorageService storageService)
        {
            _context = context;
            _storageService = storageService;
        }

        public IActionResult Index()
        {
            var reviews = _context.CustomerReviews.ToList();
            return View(reviews);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CustomerReview model, IFormFile? avatarFile, IFormFile? suitFile)
        {
            if (avatarFile != null)
            {
                model.AvatarImage = await _storageService.UploadFileAsync(avatarFile, "uploads/reviews");
            }
            if (suitFile != null)
            {
                model.SuitImage = await _storageService.UploadFileAsync(suitFile, "uploads/reviews");
            }

            model.CreatedAt = DateTime.Now;
            _context.CustomerReviews.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var review = _context.CustomerReviews.FirstOrDefault(x => x.ReviewId == id);
            if (review == null)
                return NotFound();

            return View(review);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CustomerReview model, IFormFile? avatarFile, IFormFile? suitFile)
        {
            var review = _context.CustomerReviews.FirstOrDefault(x => x.ReviewId == model.ReviewId);
            if (review == null)
                return NotFound();

            review.CustomerName = model.CustomerName;
            review.Profession = model.Profession;
            review.Country = model.Country;
            review.Rating = model.Rating;
            review.ReviewContent = model.ReviewContent;
            review.IsPublished = model.IsPublished;

            // AVATAR: Chỉ cập nhật khi người dùng chọn tải ảnh mới lên
            if (avatarFile != null)
            {
                review.AvatarImage = await _storageService.UploadFileAsync(avatarFile, "uploads/reviews");
            }

            // SUIT IMAGE: Chỉ cập nhật khi người dùng chọn tải ảnh mới lên
            if (suitFile != null)
            {
                review.SuitImage = await _storageService.UploadFileAsync(suitFile, "uploads/reviews");
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}