using Microsoft.AspNetCore.Mvc;

namespace BookingHotel.Controllers
{
    public class AdminDashboardController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}