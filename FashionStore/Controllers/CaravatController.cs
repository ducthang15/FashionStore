using FashionStore.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Controllers
{
    public class CaravatController : BaseController
    {
        public CaravatController(fashionDbContext context) : base(context)
        {
        }

        // 1. Trang danh sách (Chỉ hiện Đồng Hồ - CategoryId = 2)
        public async Task<IActionResult> Index()
        {
            // Lọc dữ liệu: Chỉ lấy sản phẩm có CategoryId == 2
            var dongHoList = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == 2) 
                .ToListAsync();
            ViewBag.CurrentCategoryId = 2;
            return View(dongHoList);
        }

        // 2. Tái sử dụng trang chi tiết (Không cần viết lại)
        // Khi khách bấm xem chi tiết, ta đẩy họ sang Home/Details/ID là xong
        // Hoặc nếu muốn giao diện khác, bạn có thể viết hàm Details riêng ở đây.
    }
}