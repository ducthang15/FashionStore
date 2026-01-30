using FashionStore.Repository;
using FashionStore.Repository.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ContactAdminController : Controller
    {
        private readonly fashionDbContext _context;

        public ContactAdminController(fashionDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Sắp xếp tin mới nhất lên đầu
            var contacts = await _context.Contacts.OrderByDescending(c => c.CreatedAt).ToListAsync();
            return View(contacts);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact != null)
            {
                _context.Contacts.Remove(contact);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}