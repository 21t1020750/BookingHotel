using BookingHotel.Areas.Admin.Data;
using BookingHotel.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingHotel.Controllers
{
    public class OffersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OffersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new OfferViewModel
            {
                Categories = await _context.Categories.ToListAsync(),
                Offers = await _context.Offers
                    .Include(o => o.Category)
                    .Include(o => o.Highlights)
                    .Where(o => o.IsActive)
                    .ToListAsync()
            };
            return View(model);
        }
    }

    public class OfferViewModel
    {
        public List<Category> Categories { get; set; }
        public List<Offer> Offers { get; set; }
    }
}