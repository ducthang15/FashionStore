using FashionStore.Repository;
using FashionStore.Repository.Models;
using Microsoft.AspNetCore.Mvc;

namespace FashionStore.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly fashionDbContext _context;

        public AppointmentController(fashionDbContext context)
        {
            _context = context;
        }

        // 1. Hiển thị Form đặt lịch
        public IActionResult Index()
        {
            return View();
        }

        // 2. Xử lý lưu thông tin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                appointment.CreatedAt = DateTime.Now;
                appointment.Status = "Mới"; // Mặc định trạng thái ban đầu

                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();

                // Chuyển sang trang thông báo thành công
                return RedirectToAction("Success");
            }
            return View("Index", appointment);
        }

        // 3. Trang thông báo thành công
        public IActionResult Success()
        {
            return View();
        }
    }
}