using FashionStore.Repository;
using Microsoft.AspNetCore.Mvc;
using FashionStore.Repository.Models;
using System.Text.Json;
using MailKit.Net.Smtp;
using MimeKit;

namespace FashionStore.Controllers
{
    public class ContactController : Controller
    {
        private readonly fashionDbContext _context;

        private readonly IConfiguration _configuration;

        public ContactController(fashionDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
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
                ModelState.AddModelError(string.Empty, "Please complete the security verification before submitting.");
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
                    ModelState.AddModelError(string.Empty, "Security verification failed or has expired; please try again.");
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

                // Gửi email thông báo cho Admin
                await SendEmailToAdmin(contact);

                TempData["SuccessMessage"] = "Message sent successfully! We will get back to you as soon as possible.";
                return RedirectToAction(nameof(Index));
            }

            return View("Index", contact);
        }
        private async Task SendEmailToAdmin(Contact contact)
        {
            try
            {
                var mailSettings = _configuration.GetSection("MailSettings");
                var message = new MimeMessage();
                message.From.Add(
                    new MailboxAddress(
                        mailSettings["DisplayName"],
                        mailSettings["Mail"]
                    )
                );

                message.To.Add(
                    new MailboxAddress(
                        "Admin UNLIM TAILOR",
                        mailSettings["AdminEmail"]
                    )
                );

                message.Subject = "[UNLIM TAILOR] THÔNG BÁO CÓ LIÊN HỆ MỚI";

                var bodyBuilder = new BodyBuilder();

                bodyBuilder.HtmlBody = $@"
            <div style='font-family: Arial, sans-serif; border: 1px solid #ddd; padding: 20px; max-width: 650px;'>
                <h2 style='color: #2c3e50;'>
                    Có khách hàng mới liên hệ
                </h2>
                <p>
                    Hệ thống UNLIM TAILOR vừa nhận được
                    một thông tin liên hệ mới:
                </p>
                <table style='width:100%; border-collapse:collapse;'>
                    <tr>
                        <td style='padding:10px; border-bottom:1px solid #eee;'>
                            <b>Họ tên:</b>
                        </td>
                        <td style='padding:10px; border-bottom:1px solid #eee;'>
                            {System.Net.WebUtility.HtmlEncode(contact.FullName)}
                        </td>
                    </tr>
                    <tr>
                        <td style='padding:10px; border-bottom:1px solid #eee;'>
                            <b>Số điện thoại:</b>
                        </td>
                        <td style='padding:10px; border-bottom:1px solid #eee;'>
                            {System.Net.WebUtility.HtmlEncode(contact.Phone)}
                        </td>
                    </tr>
                    <tr>
                        <td style='padding:10px; border-bottom:1px solid #eee;'>
                            <b>Email:</b>
                        </td>
                        <td style='padding:10px; border-bottom:1px solid #eee;'>
                            {System.Net.WebUtility.HtmlEncode(contact.Email)}
                        </td>
                    </tr>

                    <tr>
                        <td style='padding:10px; border-bottom:1px solid #eee;'>
                            <b>Nội dung:</b>
                        </td>
                        <td style='padding:10px; border-bottom:1px solid #eee;'>
                            {System.Net.WebUtility.HtmlEncode(contact.Message)}
                        </td>
                    </tr>

                    <tr>
                        <td style='padding:10px;'>
                            <b>Thời gian:</b>
                        </td>
                        <td style='padding:10px;'>
                            {contact.CreatedAt:dd/MM/yyyy HH:mm}
                        </td>
                    </tr>

                </table>

                <p style='margin-top:20px; color:#777; font-size:13px;'>
                    Email được gửi tự động từ hệ thống UNLIM TAILOR.
                </p>

            </div>";

                message.Body = bodyBuilder.ToMessageBody();
                using (var client = new SmtpClient())
                {
                    client.ServerCertificateValidationCallback =
                        (s, c, h, e) => true;

                    await client.ConnectAsync(mailSettings["Host"], int.Parse(mailSettings["Port"]),
                        MailKit.Security.SecureSocketOptions.StartTls
                    );

                    await client.AuthenticateAsync(mailSettings["Mail"], mailSettings["Password"]
                    );

                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Contact email error: " + ex.Message);
            }
        }
    }
}