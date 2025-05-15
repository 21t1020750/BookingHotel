using Microsoft.AspNetCore.Mvc;

namespace BookingHotel.Controllers
{
    public class BookingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
