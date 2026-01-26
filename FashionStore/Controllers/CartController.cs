using FashionStore.Helpers;
using FashionStore.Models;
using FashionStore.Repository;
using FashionStore.Repository.Models;
using Microsoft.AspNetCore.Mvc;

namespace FashionStore.Controllers
{
    public class CartController : Controller
    {
        private readonly fashionDbContext _context;

        public CartController(fashionDbContext context)
        {
            _context = context;
        }

        // Lấy giỏ hàng từ Session
        private List<CartItem> GetCartItems()
        {
            return HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
        }

        // 1. Giao diện hiển thị giỏ hàng
        public IActionResult Index()
        {
            var cart = GetCartItems();
            return View(cart);
        }

        // 2. Chức năng: Thêm sản phẩm vào giỏ (Liên kết với Database)
        public IActionResult AddToCart(int id)
        {
            // Tìm sản phẩm trong Database-First
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();

            var cart = GetCartItems();
            var existingItem = cart.FirstOrDefault(c => c.ProductId == id);

            if (existingItem != null)
            {
                existingItem.Quantity++; // Đã có trong giỏ thì tăng số lượng
            }
            else
            {
                // Chưa có thì tạo mới
                cart.Add(new CartItem
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    Price = product.Price,
                    ImageUrl = product.ImageUrl,
                    Quantity = 1
                });
            }

            // Lưu lại vào Session
            HttpContext.Session.SetObjectAsJson("Cart", cart);

            return RedirectToAction("Index"); // Chuyển đến trang xem Giỏ hàng
        }
        // 3. Chức năng: Xóa sản phẩm khỏi giỏ
        public IActionResult RemoveFromCart(int id)
        {
            var cart = GetCartItems();

            // Tìm và xóa sản phẩm có ProductId trùng khớp
            cart.RemoveAll(c => c.ProductId == id);

            // Lưu lại giỏ hàng mới vào Session
            HttpContext.Session.SetObjectAsJson("Cart", cart);

            return RedirectToAction("Index");
        }
        // 4. Giao diện trang Thanh toán (Điền thông tin)
        public IActionResult Checkout()
        {
            var cart = GetCartItems();
            if (cart.Count == 0)
            {
                return RedirectToAction("Index", "Home"); // Giỏ hàng trống thì về trang chủ
            }

            // Truyền giỏ hàng sang View để hiển thị lại hóa đơn
            ViewBag.Total = cart.Sum(c => c.Total);
            return View(cart);
        }
        // 5. Xử lý dữ liệu khi bấm nút Đặt Hàng (Lưu vào Database)
        [HttpPost]
        public async Task<IActionResult> Checkout(string ReceiverName, string ReceiverPhone, string ShippingAddress)
        {
            var cart = GetCartItems();
            if (cart.Count == 0) return RedirectToAction("Index", "Home");

            // BƯỚC 1: TẠO HÓA ĐƠN CHÍNH (Bảng Orders)
            var order = new Order
            {
                ReceiverName = ReceiverName,
                ReceiverPhone = ReceiverPhone,
                ShippingAddress = ShippingAddress,
                OrderDate = DateTime.Now,
                TotalAmount = cart.Sum(c => c.Total),
                OrderStatus = "Chờ xác nhận",
                PaymentMethod = "Thanh toán khi nhận hàng (COD)"
            };

            // Lưu Hóa đơn vào Database để hệ thống tạo OrderId tự động
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // BƯỚC 2: LƯU TỪNG MÓN HÀNG TRONG GIỎ (Bảng OrderDetails)
            foreach (var item in cart)
            {
                var orderDetail = new OrderDetail
                {
                    OrderId = order.OrderId, // Lấy ID của hóa đơn vừa tạo ở trên
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Price // Lưu giá tại thời điểm mua
                };
                _context.OrderDetails.Add(orderDetail);
            }

            await _context.SaveChangesAsync(); // Lưu tất cả chi tiết

            // BƯỚC 3: XÓA SẠCH GIỎ HÀNG TRONG SESSION
            HttpContext.Session.Remove("Cart");

            // BƯỚC 4: CHUYỂN ĐẾN TRANG THÔNG BÁO THÀNH CÔNG
            return RedirectToAction("OrderSuccess");
        }

        // 6. Trang thông báo Đặt hàng thành công
        public IActionResult OrderSuccess()
        {
            return View();
        }
    }
}