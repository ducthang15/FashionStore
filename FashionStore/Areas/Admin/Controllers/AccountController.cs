using FashionStore.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FashionStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly fashionDbContext _context;

        public AccountController(fashionDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string Username, string Password)
        {
            string passwordHash = GetMD5(Password);

            var user = _context.Users
                .FirstOrDefault(u => u.Username == Username
                                  && u.PasswordHash == passwordHash
                                  && u.Role == "Admin"); // 👈 CHỈ ADMIN

            if (user == null)
            {
                ViewBag.Error = "Không phải tài khoản Admin";
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("UserId", user.UserId.ToString())
            };

            var identity = new ClaimsIdentity(claims, "AdminScheme");
            await HttpContext.SignInAsync(
                "AdminScheme",
                new ClaimsPrincipal(identity));

            return RedirectToAction("Index", "ProductAdmin");
        }
        public static string GetMD5(string str)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] fromData = Encoding.UTF8.GetBytes(str);
                byte[] targetData = md5.ComputeHash(fromData);

                StringBuilder sb = new StringBuilder();
                foreach (byte b in targetData)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }
    }
}