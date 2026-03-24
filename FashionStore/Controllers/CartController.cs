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

        private List<CartItem> GetCartItems()
        {
            return HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
        }

        public IActionResult Index()
        {
            var cart = GetCartItems();
            return View(cart);
        }

        public IActionResult AddToCart(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();

            var cart = GetCartItems();
            var existingItem = cart.FirstOrDefault(c => c.ProductId == id);

            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    Price = product.Price,
                    ImageUrl = product.ImageUrl,
                    Quantity = 1
                });
            }
            HttpContext.Session.SetObjectAsJson("Cart", cart);

            return RedirectToAction("Index");
        }
        public IActionResult RemoveFromCart(int id)
        {
            var cart = GetCartItems();
            cart.RemoveAll(c => c.ProductId == id);
            HttpContext.Session.SetObjectAsJson("Cart", cart);

            return RedirectToAction("Index");
        }
        public IActionResult Checkout()
        {
            var cart = GetCartItems();
            if (cart.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Total = cart.Sum(c => c.Total);
            return View(cart);
        }
        [HttpPost]
        public async Task<IActionResult> Checkout(string ReceiverName, string ReceiverPhone, string ShippingAddress)
        {
            var cart = GetCartItems();
            if (cart.Count == 0) return RedirectToAction("Index", "Home");

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

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            foreach (var item in cart)
            {
                var orderDetail = new OrderDetail
                {
                    OrderId = order.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Price
                };
                _context.OrderDetails.Add(orderDetail);
            }

            await _context.SaveChangesAsync();
            HttpContext.Session.Remove("Cart");
            return RedirectToAction("OrderSuccess");
        }
        public IActionResult OrderSuccess()
        {
            return View();
        }
    }
}