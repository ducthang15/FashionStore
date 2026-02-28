using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using FashionStore.Repository.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FashionStore.Repository;
using System.Text;

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

            if (user.PasswordHash != ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Mật khẩu xác nhận không khớp.");
                return View(user);
            }
            user.PasswordHash = GetMD5(user.PasswordHash);
            user.Role = "Customer"; // Mặc định là khách hàng

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }

        // --- ĐĂNG NHẬP ---
        public IActionResult Login(string returnUrl = "/")
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string Username, string Password, string returnUrl = "/")
        {
            string passwordHash = GetMD5(Password);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == Username && u.PasswordHash == passwordHash && u.Role == "Customer");

            if (user == null)
            {
                ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu";
                return View();
            }

            // Tạo danh sách quyền hạn (Claims)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FullName), 
                new Claim(ClaimTypes.Role, user.Role),     
                new Claim("UserId", user.UserId.ToString()) 
            };

            var claimsIdentity = new ClaimsIdentity(claims, "CustomerScheme");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync("CustomerScheme", new ClaimsPrincipal(claimsIdentity));

            if (user.Role == "Admin")
            {
                return RedirectToAction("Index", "ProductAdmin", new { area = "Admin" });
            }

            return Redirect(returnUrl);
        }

        // --- ĐĂNG XUẤT ---
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CustomerScheme");
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