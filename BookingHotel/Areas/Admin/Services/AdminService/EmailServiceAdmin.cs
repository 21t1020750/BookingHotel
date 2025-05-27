using BookingHotel.Areas.Admin.Services;
using BookingHotel.Models;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace BookingHotel.Areas.Admin.Services.AdminService
{
    public class EmailServiceAdmin : IEmailServiceAdmin
    {
        private readonly EmailSettings _emailSettings;

        public EmailServiceAdmin(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                using var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
                {
                    Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                // Ghi log lỗi (sử dụng ILogger nếu có)
                throw new Exception($"Không thể gửi email: {ex.Message}", ex);
            }
        }
    }
}
