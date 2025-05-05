using Microsoft.AspNetCore.Mvc;

namespace BookingHotel.Controllers
{
    public class RoomsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
