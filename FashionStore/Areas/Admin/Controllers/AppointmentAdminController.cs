using FashionStore.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace FashionStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] // Bảo vệ: Chỉ Admin mới được vào
    public class AppointmentAdminController : Controller
    {
        private readonly fashionDbContext _context;

        public AppointmentAdminController(fashionDbContext context)
        {
            _context = context;
        }

        // 1. Xem danh sách lịch hẹn (Mới nhất lên đầu)
        public async Task<IActionResult> Index()
        {
            var appointments = await _context.Appointments
                .OrderByDescending(a => a.CreatedAt) // Sắp xếp theo ngày tạo mới nhất
                .ToListAsync();

            return View(appointments);
        }

        // 2. Cập nhật trạng thái (Ví dụ: Bấm nút "Xác nhận")
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                appointment.Status = status; // Cập nhật trạng thái mới
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // 3. Xóa lịch hẹn (Nếu bị spam)
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