using Microsoft.AspNetCore.Mvc;

namespace BookingHotel.Controllers
{
    public class OffersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
