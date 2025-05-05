using Microsoft.AspNetCore.Mvc;

namespace BookingHotel.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
