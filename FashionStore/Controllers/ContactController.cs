using FashionStore.Repository;
using Microsoft.AspNetCore.Mvc;
using FashionStore.Repository.Models;

namespace FashionStore.Controllers
{
    public class ContactController : Controller
    {
        private readonly fashionDbContext _context;

        public ContactController(fashionDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Send(Contact contact)
        {
            if (ModelState.IsValid)
            {
                contact.CreatedAt = DateTime.Now;
                contact.IsRead = false;

                _context.Add(contact);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Gửi tin nhắn thành công! Chúng tôi sẽ liên hệ lại sớm nhất.";
                return RedirectToAction(nameof(Index));
            }
            return View("Index", contact);
        }
    }
}