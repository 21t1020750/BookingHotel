using BookingHotel.Areas.Admin.Data;
using BookingHotel.Areas.Admin.Models;
using BookingHotel.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookingHotel.Controllers
{
    public class AccountController(ApplicationDbContext _context) : Controller
    {
        private const string DefaultPhoto = "nophoto.png";
        [HttpGet]
        public IActionResult LoginRegister()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, string tab ="login")
        {
            // Kiểm tra Employee trước
            var employee = _context.Employees.FirstOrDefault(e => e.Email == email && e.Password == password);
            if (employee != null)
            {
                // Bước 1: Tạo danh sách các claims
                var claims = new List<Claim>
        {
                new Claim(ClaimTypes.NameIdentifier, employee.EmployeeID.ToString()),
                new Claim(ClaimTypes.Name, employee.FullName),
                new Claim(ClaimTypes.Email, employee.Email),
                new Claim("Photo", employee.Photo),
                new Claim(ClaimTypes.Role, employee.Roles),
                new Claim("EmployeeID", employee.EmployeeID.ToString())
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
                new Claim(ClaimTypes.NameIdentifier, customer.CustomerID.ToString()),
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

            ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng.");
            ViewBag.Tab = tab;
            return View("LoginRegister");
        }

        [HttpGet]
        public IActionResult Register(string tab = "register")
        {
            ViewBag.Tab = tab;
            return View("LoginRegister");
        }


        [HttpPost]
        public async Task<IActionResult> Register(RegisterUser model, string tab = "register")
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Tab = tab;
                return View("LoginRegister", model);
            }

            if (!model.AgreeTerms)
            {
                ModelState.AddModelError("AgreeTerms", "Bạn cần đồng ý với điều khoản.");
                ViewBag.Tab = tab;
                return View("LoginRegister", model);
            }

            var exists = _context.Customers.Any(c => c.Email == model.Email);
            if (exists)
            {
                ModelState.AddModelError("Email", "Email đã được đăng ký.");
                ViewBag.Tab = tab;
                return View("LoginRegister", model);
            }

            var customer = new Customer
            {
                FullName = model.FullName,
                Email = model.Email,
                Phone = model.Phone,
                Password = model.Password,
                DateOfBirth = model.DateOfBirth,
                Address = model.Address,
                CreatedAt = DateTime.Now,
                Photo = DefaultPhoto
            };

            try
            {
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi DB: " + ex.Message);
                ModelState.AddModelError("", "Không thể lưu dữ liệu vào database.");
                ViewBag.Tab = tab;
                return View("LoginRegister", model);
            }

            // Đăng nhập sau khi đăng ký thành công
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, customer.CustomerID.ToString()),
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
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (role == "employee")
                return RedirectToAction("AccessDenied", "AdminDashBoard", new { area = "Admin" });

            if (role == "Customer")
                return RedirectToAction("AccessDenied", "Account");


            return View();
        }
    }
}
