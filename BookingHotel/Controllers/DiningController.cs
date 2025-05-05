using Microsoft.AspNetCore.Mvc;

namespace BookingHotel.Controllers
{
    public class DiningController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
