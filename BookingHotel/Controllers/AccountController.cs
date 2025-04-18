using Microsoft.AspNetCore.Mvc;

namespace BookingHotel.Controllers
{
    public class AccountController : Controller
    {
        // Hiển thị trang đăng nhập
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Xử lý form đăng nhập
        [HttpPost]
        public IActionResult Login(string emailOrPhone, string password)
        {
            // Logic xác thực giả lập
            if (!string.IsNullOrEmpty(emailOrPhone) && !string.IsNullOrEmpty(password))
            {
                // Giả lập đăng nhập thành công
                return RedirectToAction("WebBooking", "Home");
            }

            // Nếu thất bại, hiển thị lại form với lỗi
            ViewBag.ErrorMessage = "Email/Số điện thoại hoặc mật khẩu không đúng.";
            return View();
        }
    }
}