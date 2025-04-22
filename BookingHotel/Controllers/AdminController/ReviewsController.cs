using BookingHotel.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingHotel.Controllers.AdminController
{
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var reviews = _context.Reviews
                .Include(r => r.Booking)
                .Include(r => r.Customer)
                .ToList();
            return View(reviews);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var review = _context.Reviews
                .Include(b => b.Customer)
                .Include(b => b.Booking)
                .FirstOrDefault(b => b.ReviewID == id);

            if (review == null)
            {
                return NotFound();
            }

            return PartialView(review);
        }
    }
}
