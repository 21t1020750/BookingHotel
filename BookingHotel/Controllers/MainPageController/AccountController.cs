using BookingHotel.Data;
using Microsoft.AspNetCore.Mvc;

namespace BookingHotel.Controllers.MainPageController
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            // Tìm trong Employee trước
            var employee = _context.Employees
                            .FirstOrDefault(e => e.Email == email && e.Password == password);
            if (employee != null)
            {
                HttpContext.Session.SetString("Role", "Admin");
                HttpContext.Session.SetString("Email", employee.Email);
                return RedirectToAction("Index", "AdminDashboard");
            }

            // Tìm trong Customer
            var customer = _context.Customers
                            .FirstOrDefault(c => c.Email == email && c.Password == password);
            if (customer != null)
            {
                HttpContext.Session.SetString("Role", "Customer");
                HttpContext.Session.SetString("Email", customer.Email);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Email hoặc mật khẩu không đúng!";
            return View();
        }
    }
}
