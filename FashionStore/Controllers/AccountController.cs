using FashionStore.Repository;
using FashionStore.Repository.Models;
using FashionStore.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
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
            user.PasswordHash = PasswordHelper.Hash(user.PasswordHash);
            user.Role = "Customer"; // Mặc định là khách hàng

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }
        public IActionResult Login(string returnUrl = "/")
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string Username, string Password, string returnUrl = "/")
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == Username && u.Role == "Customer");

            if (user == null)
            {
                ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu";
                return View();
            }
            if (!PasswordHelper.Verify(user.PasswordHash, Password))
            {
                // check MD5 cũ
                string md5 = PasswordHelper.GetMD5(Password);

                if (user.PasswordHash != md5)
                {
                    ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu";
                    return View();
                }
                user.PasswordHash = PasswordHelper.Hash(Password);
                await _context.SaveChangesAsync();
            }
            if (user == null)
            {
                ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu";
                return View();
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FullName ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Role ??string.Empty),
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
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CustomerScheme");
            return RedirectToAction("Index", "Home");
        }
    }
}