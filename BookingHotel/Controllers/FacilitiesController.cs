using Microsoft.AspNetCore.Mvc;

namespace BookingHotel.Controllers
{
    public class FacilitiesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
