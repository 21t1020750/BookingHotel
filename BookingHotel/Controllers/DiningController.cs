using Microsoft.AspNetCore.Mvc;
using BookingHotel.Areas.Admin.Data;
using BookingHotel.Areas.Admin.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingHotel.Controllers
{
    public class DiningController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DiningController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Truy vấn HeroImage (chỉ lấy 1 bản ghi đầu tiên)
            var heroImage = _context.HeroImages.FirstOrDefault();

            // Truy vấn Restaurants và Tags
            var restaurants = _context.Restaurants.ToList();
            var restaurantViewModels = new List<RestaurantViewModel>();

            foreach (var restaurant in restaurants)
            {
                var tags = _context.Restaurant_Tags
                    .Where(rt => rt.RestaurantID == restaurant.RestaurantID)
                    .Join(_context.Tags,
                          rt => rt.TagID,
                          t => t.TagID,
                          (rt, t) => t.TagName)
                    .ToList();

                restaurantViewModels.Add(new RestaurantViewModel
                {
                    Restaurant = restaurant,
                    Tags = tags
                });
            }

            // Truy vấn Dishes
            var dishes = _context.Dishes.ToList();

            // Tạo ViewModel
            var viewModel = new DiningViewModel
            {
                HeroImage = heroImage,
                Restaurants = restaurantViewModels,
                Dishes = dishes
            };

            return View(viewModel);
        }
    }
}