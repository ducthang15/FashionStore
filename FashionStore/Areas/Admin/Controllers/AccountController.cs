using FashionStore.Repository;
using FashionStore.Utilities;
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
            var user = _context.Users.FirstOrDefault(u => u.Username == Username && u.Role == "Admin");

            if (user == null)
            {
                ViewBag.Error = "Not Admin";
                return View();
            }

            if (!PasswordHelper.Verify(user.PasswordHash, Password))
            {
                string md5 = PasswordHelper.GetMD5(Password);

                if (user.PasswordHash != md5)
                {
                    ViewBag.Error = "Incorrect password!";
                    return View();
                }

                // upgrade
                user.PasswordHash = PasswordHelper.Hash(Password);
                _context.SaveChanges();
            }
            if (user == null)
            {
                ViewBag.Error = "Not Admin";
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FullName ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Role ?? string.Empty),
                new Claim("UserId", user.UserId.ToString())
            };

            var identity = new ClaimsIdentity(claims, "AdminScheme");
            await HttpContext.SignInAsync(
                "AdminScheme",
                new ClaimsPrincipal(identity));

            return RedirectToAction("Index", "ProductAdmin");
        }
    }
}