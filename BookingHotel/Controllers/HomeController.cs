using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BookingHotel.Areas.Admin.Data; // Update to use ApplicationDbContext
using BookingHotel.Models;

namespace BookingHotel.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db; // Change to ApplicationDbContext

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var model = new HomeViewModel
                {
                    BannerImages = await _db.Content_BannerImages.ToListAsync(),
                    Services = await _db.Content_Services.ToListAsync(),
                    Rooms = await _db.Content_Rooms.ToListAsync(),
                    Amenities = await _db.Content_Amenities.ToListAsync(),
                    Offers = await _db.Content_Offers.ToListAsync(),
                    MembershipBenefits = await _db.Content_MembershipBenefits.ToListAsync()
                };
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while loading data for Home page");
                return View("Error");
            }
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