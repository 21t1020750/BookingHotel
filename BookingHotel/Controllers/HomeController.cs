using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BookingHotel.Controllers
{
    public class HomeController(ILogger<HomeController> _logger) : Controller
    {
        public IActionResult Index()
        {
            _ = _logger;
            return View("WebBooking");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
