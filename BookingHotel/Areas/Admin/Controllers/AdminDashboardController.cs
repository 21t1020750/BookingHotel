using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingHotel.Areas.Admin.Controllers
{
    [Area("Admin")]

    [Authorize(Roles = "Admin, employee")]
    public class AdminDashboardController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AccessDenied() 
        {
            return View();
        }
    }
}