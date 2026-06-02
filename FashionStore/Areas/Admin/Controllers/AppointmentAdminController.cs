using FashionStore.Repository;
using FashionStore.Repository.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Areas.Admin.Controllers
{
    public class AppointmentAdminController : BaseAdminController
    {
        private readonly fashionDbContext _context;

        public AppointmentAdminController(fashionDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.Reviews = _context.CustomerReviews.Where(x => x.IsPublished).OrderBy(x => x.DisplayOrder).ToList();
            return View(new Appointment());
        }
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                appointment.Status = status;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}