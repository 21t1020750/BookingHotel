using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BookingHotel.Areas.Admin.Data; // Update to use ApplicationDbContext
using BookingHotel.Models;
using iText.Commons.Actions.Contexts;

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
                    Achivements = await _db.Content_Achivements.ToListAsync(),
                    Rooms = await _db.Content_Rooms.Include(r => r.RoomType).ToListAsync(),
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

        [HttpGet]
        public IActionResult Details(int id)
        {
            var review = _db.Content_BannerImages
                .FirstOrDefault(b =>b.Id == id);

            if (review == null)
            {
                return NotFound();
            }

            return PartialView(review);
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