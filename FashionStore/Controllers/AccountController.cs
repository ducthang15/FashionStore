using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FashionStore.Repository;
using FashionStore.Repository.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly fashionDbContext _context;

        public AccountController(fashionDbContext context)
        {
            _context = context;
        }

        // --- ĐĂNG KÝ ---
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user, string ConfirmPassword)
        {
            if (_context.Users.Any(u => u.Username == user.Username))
            {
                ModelState.AddModelError("Username", "Tên đăng nhập này đã tồn tại.");
                return View(user);
            }

            if (user.PasswordHash != ConfirmPassword) // Tạm dùng biến PasswordHash để hứng mật khẩu người dùng nhập
            {
                ModelState.AddModelError("ConfirmPassword", "Mật khẩu xác nhận không khớp.");
                return View(user);
            }

            // Mã hóa mật khẩu trước khi lưu (MD5)
            user.PasswordHash = GetMD5(user.PasswordHash);
            user.Role = "Customer"; // Mặc định là khách hàng

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }

        // --- ĐĂNG NHẬP ---
        public IActionResult Login(string returnUrl = "/")
        {
            ViewBag.ReturnUrl = returnUrl; // Lưu lại link người dùng muốn vào trước đó
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string Username, string Password, string returnUrl = "/")
        {
            // Mã hóa mật khẩu nhập vào để so sánh với Database
            string passwordHash = GetMD5(Password);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == Username && u.PasswordHash == passwordHash);

            if (user == null)
            {
                ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu";
                return View();
            }

            // Tạo danh sách quyền hạn (Claims)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FullName), // Lưu tên để hiển thị
                new Claim(ClaimTypes.Role, user.Role),     // Lưu quyền (Admin hay Customer)
                new Claim("UserId", user.UserId.ToString()) // Lưu ID để dùng cho giỏ hàng sau này
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Ghi nhận đăng nhập (Tạo Cookie)
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

            // Kiểm tra role để chuyển hướng
            if (user.Role == "Admin")
            {
                return RedirectToAction("Index", "ProductAdmin", new { area = "Admin" });
            }

            return Redirect(returnUrl);
        }

        // --- ĐĂNG XUẤT ---
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // Hàm mã hóa MD5 đơn giản
        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");
            }
            return byte2String;
        }
    }
}