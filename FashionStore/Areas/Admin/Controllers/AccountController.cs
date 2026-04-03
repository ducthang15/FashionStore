using FashionStore.Repository;
using FashionStore.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FashionStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly fashionDbContext _context;
        private static Dictionary<string, (int Count, DateTime LockUntil)> loginAttempts
            = new Dictionary<string, (int, DateTime)>();

        public AccountController(fashionDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> Login(string Username, string Password)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var key = $"{Username}_{ip}";
            if (loginAttempts.ContainsKey(key))
            {
                var info = loginAttempts[key];
                if (info.LockUntil > DateTime.Now)
                {
                    ViewBag.Error = "Too many attempts. Try again in 5 minutes.";
                    return View();
                }
            }

            var user = _context.Users
                .FirstOrDefault(u => u.Username == Username && u.Role == "Admin");

            if (user == null)
            {
                IncreaseFail(key);
                ViewBag.Error = "Not Admin";
                return View();
            }
            if (!PasswordHelper.Verify(user.PasswordHash, Password))
            {
                string md5 = PasswordHelper.GetMD5(Password);

                if (user.PasswordHash != md5)
                {
                    IncreaseFail(key);
                    ViewBag.Error = "Incorrect password!";
                    return View();
                }
                user.PasswordHash = PasswordHelper.Hash(Password);
                _context.SaveChanges();
            }
            loginAttempts.Remove(key);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FullName ?? ""),
                new Claim(ClaimTypes.Role, user.Role ?? ""),
                new Claim("UserId", user.UserId.ToString())
            };

            var identity = new ClaimsIdentity(claims, "AdminScheme");

            await HttpContext.SignInAsync(
                "AdminScheme",
                new ClaimsPrincipal(identity));

            return RedirectToAction("Index", "ProductAdmin");
        }

        private void IncreaseFail(string key)
        {
            if (!loginAttempts.ContainsKey(key))
            {
                loginAttempts[key] = (1, DateTime.MinValue);
            }
            else
            {
                var current = loginAttempts[key];
                current.Count++;

                if (current.Count >= 5)
                {
                    current.LockUntil = DateTime.Now.AddMinutes(5);
                    current.Count = 0;
                }

                loginAttempts[key] = current;
            }
        }
    }
}