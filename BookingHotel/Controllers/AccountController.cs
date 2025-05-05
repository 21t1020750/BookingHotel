using System.Security.Claims;
using BookingHotel.Areas.Admin.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace BookingHotel.Controllers
{
    public class AccountController(ApplicationDbContext _context) : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // Kiểm tra Employee trước
            var employee = _context.Employees.FirstOrDefault(e => e.Email == email && e.Password == password);
            if (employee != null)
            {
                // Bước 1: Tạo danh sách các claims
                var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, employee.FullName),
        new Claim(ClaimTypes.Email, employee.Email),
        new Claim("Photo", employee.Photo),
        new Claim(ClaimTypes.Role, "Admin")
    };

                // Bước 2: Tạo Identity từ Claims
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Bước 3: Tạo Principal từ Identity
                var principal = new ClaimsPrincipal(identity);

                // Bước 4: Đăng nhập (Sign In)
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("Index", "AdminDashboard", new { area = "Admin" });
            }

            // Kiểm tra Customer
            var customer = _context.Customers.FirstOrDefault(c => c.Email == email && c.Password == password);
            if (customer != null)
            {
                var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, customer.FullName),
        new Claim(ClaimTypes.Email, customer.Email),
        new Claim("Photo", customer.Photo),
        new Claim(ClaimTypes.Role, "Customer")
    };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Email hoặc mật khẩu không đúng!";
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            // SignOutAsync sẽ tự động xóa Authentication Cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Sau khi đăng xuất thì chuyển về trang Login hoặc Home
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
