using BookingHotel.Models;
using BookingHotel.Services;
using Microsoft.AspNetCore.Mvc;

public class ContactController : Controller
{
    private readonly IEmailService _emailService;

    public ContactController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ContactFormModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                string subject = $"[Liên hệ] {model.Subject}";
                string body = $@"
                    <p><strong>Tên:</strong> {model.Name}</p>
                    <p><strong>Email:</strong> {model.Email}</p>
                    <p><strong>Số điện thoại:</strong> {model.Phone}</p>
                    <p><strong>Nội dung:</strong></p>
                    <p>{model.Message}</p>";

                // Gửi về email của admin (hoặc chính mình)
                await _emailService.SendEmailAsync("huyleviet666@gmail.com", subject, body);

                TempData["Success"] = "Tin nhắn đã được gửi thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Lỗi gửi email: " + ex.Message);
            }
        }

        return View(model);
    }
}
