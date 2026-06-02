using FashionStore.Repository;
using FashionStore.Repository.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace FashionStore.Areas.Admin.Controllers
{
    public class CustomerReviewController : BaseAdminController
    {
        private readonly fashionDbContext _context;

        public CustomerReviewController(fashionDbContext context)
        {
            _context = context;
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
        public IActionResult Edit(int id)
        {
            var review = _context.CustomerReviews.FirstOrDefault(x => x.ReviewId == id);

            if (review == null)
                return NotFound();

            return View(review);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(CustomerReview model, IFormFile avatarFile, IFormFile suitFile)
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

            // AVATAR
            if (avatarFile != null)
            {
                string fileName = Guid.NewGuid() + Path.GetExtension(avatarFile.FileName);
                string path = Path.Combine(Directory.GetCurrentDirectory(),
                    "wwwroot/uploads/reviews", fileName);

                using var stream = new FileStream(path, FileMode.Create);
                await avatarFile.CopyToAsync(stream);

                review.AvatarImage = "/uploads/reviews/" + fileName;
            }

            // SUIT IMAGE
            if (suitFile != null)
            {
                string fileName = Guid.NewGuid() + Path.GetExtension(suitFile.FileName);
                string path = Path.Combine(Directory.GetCurrentDirectory(),
                    "wwwroot/uploads/reviews", fileName);

                using var stream = new FileStream(path, FileMode.Create);
                await suitFile.CopyToAsync(stream);

                review.SuitImage = "/uploads/reviews/" + fileName;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> Create(CustomerReview model, IFormFile avatarFile, IFormFile suitFile)
        {
            if (avatarFile != null)
            {
                string fileName = Guid.NewGuid() + Path.GetExtension(avatarFile.FileName);
                string path = Path.Combine( Directory.GetCurrentDirectory(), "wwwroot/uploads/reviews",fileName);
                using var stream = new FileStream(path, FileMode.Create);
                await avatarFile.CopyToAsync(stream);
                model.AvatarImage = "/uploads/reviews/" + fileName;
            }
            if (suitFile != null)
            {
                string fileName = Guid.NewGuid() +Path.GetExtension(suitFile.FileName);
                string path = Path.Combine( Directory.GetCurrentDirectory(), "wwwroot/uploads/reviews", fileName);
                using var stream = new FileStream(path, FileMode.Create);
                await suitFile.CopyToAsync(stream);
                model.SuitImage = "/uploads/reviews/" + fileName;
            }

            model.CreatedAt = DateTime.Now;
            _context.CustomerReviews.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}