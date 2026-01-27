using FashionStore.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderAdminController : Controller
    {
        private readonly fashionDbContext _context;

        public OrderAdminController(fashionDbContext context)
        {
            _context = context;
        }

        // 1. Danh sách đơn hàng (Mới nhất lên đầu)
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .OrderByDescending(o => o.OrderDate) // Sắp xếp ngày mới nhất lên đầu
                .ToListAsync();
            return View(orders);
        }

        // 2. Xem chi tiết đơn hàng
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            // Lấy đơn hàng + Chi tiết + Thông tin sản phẩm trong chi tiết đó
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product) // Kỹ thuật Eager Loading: Lấy luôn Product từ OrderDetail
                .FirstOrDefaultAsync(m => m.OrderId == id);

            if (order == null) return NotFound();

            return View(order);
        }

        // 3. Xóa đơn hàng (Nếu cần)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                // Phải xóa OrderDetails trước (do ràng buộc khóa ngoại)
                var details = _context.OrderDetails.Where(od => od.OrderId == id);
                _context.OrderDetails.RemoveRange(details);

                // Sau đó mới xóa Order
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}