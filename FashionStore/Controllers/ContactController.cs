using FashionStore.Repository;
using Microsoft.AspNetCore.Mvc;
using FashionStore.Repository.Models;
using System.Text.Json;

namespace FashionStore.Controllers
{
    public class ContactController : Controller
    {
        private readonly fashionDbContext _context;

        public ContactController(fashionDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(Contact contact)
        {
            // BƯỚC 1: Lấy mã xác thực Turnstile từ form gửi lên
            var turnstileResponse = Request.Form["cf-turnstile-response"].ToString();
            var secretKey = "0x4AAAAAAElum4cKRC8ngtFgJvP5vd5YsPI";

            // Nếu không có mã phản hồi
            if (string.IsNullOrEmpty(turnstileResponse))
            {
                ModelState.AddModelError(string.Empty, "Vui lòng xác minh bảo mật trước khi gửi.");
                return View("Index", contact);
            }

            // Gửi sang Cloudflare để kiểm tra
            using (var httpClient = new HttpClient())
            {
                var postData = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("secret", secretKey),
                    new KeyValuePair<string, string>("response", turnstileResponse)
                });

                var res = await httpClient.PostAsync("https://challenges.cloudflare.com/turnstile/v0/siteverify", postData);
                var json = await res.Content.ReadAsStringAsync();

                using var doc = JsonDocument.Parse(json);
                bool isSuccess = doc.RootElement.GetProperty("success").GetBoolean();

                // Nếu Cloudflare báo bot hoặc token sai
                if (!isSuccess)
                {
                    ModelState.AddModelError(string.Empty, "Xác minh bảo mật thất bại hoặc đã quá hạn, vui lòng thử lại.");
                    return View("Index", contact);
                }
            }

            // BƯỚC 2: Khi xác thực hợp lệ -> Thực hiện lưu dữ liệu vào Database
            if (ModelState.IsValid)
            {
                contact.CreatedAt = DateTime.Now;
                contact.IsRead = false;

                _context.Add(contact);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Gửi tin nhắn thành công! Chúng tôi sẽ liên hệ lại sớm nhất.";
                return RedirectToAction(nameof(Index));
            }

            return View("Index", contact);
        }
    }
}