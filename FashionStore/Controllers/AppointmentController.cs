using FashionStore.Repository.Models;
using FashionStore.Repository;
using Microsoft.AspNetCore.Mvc;
using MailKit.Net.Smtp;
using MimeKit;          
using Microsoft.Extensions.Configuration; 

namespace FashionStore.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly fashionDbContext _context;
        private readonly IConfiguration _configuration;

        public AppointmentController(fashionDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Book(Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                appointment.CreatedAt = DateTime.Now;
                appointment.Status = "Mới";

                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();
                await SendEmailToAdmin(appointment);

                return RedirectToAction("Success");
            }
            return View("Index", appointment);
        }

        private async Task SendEmailToAdmin(Appointment appointment)
        {
            try
            {
                // Lấy cấu hình từ appsettings.json
                var mailSettings = _configuration.GetSection("MailSettings");

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(mailSettings["DisplayName"], mailSettings["Mail"]));

                // Email nhận thông báo (Có thể dùng chính mail gửi hoặc mail khác)
                message.To.Add(new MailboxAddress("Admin UNLIM TAILOR", "nguyenthangbe04@gmail.com"));

                message.Subject = "[UNLIM TAILOR] THÔNG BÁO CÓ LỊCH HẸN MỚI";

                var bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = $@"
                    <div style='font-family: sans-serif; border: 1px solid #ddd; padding: 20px; max-width: 600px;'>
                        <h2 style='color: #2c3e50;'>Yêu cầu đặt lịch hẹn mới</h2>
                        <p>Chào Admin, hệ thống vừa ghi nhận một yêu cầu từ khách hàng:</p>
                        <table style='width: 100%; border-collapse: collapse;'>
                            <tr>
                                <td style='padding: 8px; border-bottom: 1px solid #eee;'><b>Khách hàng:</b></td>
                                <td style='padding: 8px; border-bottom: 1px solid #eee;'>{appointment.FullName}</td>
                            </tr>
                            <tr>
                                <td style='padding: 8px; border-bottom: 1px solid #eee;'><b>Điện thoại:</b></td>
                                <td style='padding: 8px; border-bottom: 1px solid #eee;'>{appointment.PhoneNumber}</td>
                            </tr>
                            <tr>
                                <td style='padding: 8px; border-bottom: 1px solid #eee;'><b>Ngày hẹn:</b></td>
                                <td style='padding: 8px; border-bottom: 1px solid #eee;'>{appointment.AppointmentDate:dd/MM/yyyy HH:mm}</td>
                            </tr>
                            <tr>
                                <td style='padding: 8px; border-bottom: 1px solid #eee;'><b>Địa chỉ:</b></td>
                                <td style='padding: 8px; border-bottom: 1px solid #eee;'>{appointment.Address}</td>
                            </tr>
                            <tr>
                                <td style='padding: 8px; border-bottom: 1px solid #eee;'><b>Nội dung:</b></td>
                                <td style='padding: 8px; border-bottom: 1px solid #eee;'>{appointment.Content}</td>
                            </tr>
                        </table>
                    </div>";

                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    await client.ConnectAsync(mailSettings["Host"], int.Parse(mailSettings["Port"]), MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(mailSettings["Mail"], mailSettings["Password"]);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception)
            {

            }
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}