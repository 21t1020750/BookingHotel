using BookingHotel.Areas.Admin.Data;
using BookingHotel.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingHotel.Areas.Admin.Controllers
{
    [Area("Admin")]

    [Authorize(Roles = "Admin, employee")]
    public class AdminOffersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminOffersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // List all offers
        public async Task<IActionResult> Index()
        {
            var offers = await _context.Offers
                .Include(o => o.Category)
                .Include(o => o.Highlights)
                .ToListAsync();
            return View(offers);
        }

        // Create new offer (GET)
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View();
        }

        // Create new offer (POST)
        [HttpPost]
        public async Task<IActionResult> Create(Offer offer, List<string> Highlights)
        {
            if (ModelState.IsValid)
            {
                offer.CreatedAt = DateTime.Now;
                offer.UpdatedAt = DateTime.Now;
                _context.Offers.Add(offer);
                await _context.SaveChangesAsync();

                foreach (var highlightText in Highlights)
                {
                    if (!string.IsNullOrEmpty(highlightText))
                    {
                        _context.OfferHighlights.Add(new OfferHighlight
                        {
                            OfferId = offer.OfferId,
                            HighlightText = highlightText,
                            IconClass = "fas fa-check",
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        });
                    }
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(offer);
        }

        // Edit offer (GET)
        public async Task<IActionResult> Edit(int id)
        {
            var offer = await _context.Offers
                .Include(o => o.Highlights)
                .FirstOrDefaultAsync(o => o.OfferId == id);
            if (offer == null)
            {
                return NotFound();
            }
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(offer);
        }

        // Edit offer (POST)
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Offer offer, List<string> Highlights)
        {
            if (id != offer.OfferId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Update offer
                    var existingOffer = await _context.Offers
                        .Include(o => o.Highlights)
                        .FirstOrDefaultAsync(o => o.OfferId == id);
                    if (existingOffer == null)
                    {
                        return NotFound();
                    }

                    // Update fields
                    existingOffer.Title = offer.Title;
                    existingOffer.Description = offer.Description;
                    existingOffer.CategoryId = offer.CategoryId;
                    existingOffer.DiscountPercentage = offer.DiscountPercentage;
               
                  
                    existingOffer.ValidUntil = offer.ValidUntil;
                    existingOffer.IconClass = offer.IconClass;
                    existingOffer.AltText = offer.AltText;
                    existingOffer.UpdatedAt = DateTime.Now; // Ensure valid datetime
                    existingOffer.IsActive = offer.IsActive;

                    // Update highlights
                    _context.OfferHighlights.RemoveRange(existingOffer.Highlights);
                    foreach (var highlightText in Highlights)
                    {
                        if (!string.IsNullOrEmpty(highlightText))
                        {
                            _context.OfferHighlights.Add(new OfferHighlight
                            {
                                OfferId = existingOffer.OfferId,
                                HighlightText = highlightText,
                                IconClass = "fas fa-check",
                                CreatedAt = DateTime.Now, // Ensure valid datetime
                                UpdatedAt = DateTime.Now // Ensure valid datetime
                            });
                        }
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateException ex)
                {
                    // Log the inner exception for debugging
                    ModelState.AddModelError("", $"Error updating offer: {ex.InnerException?.Message}");
                }
            }
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(offer);
        }

        // Delete offer (GET)
        public async Task<IActionResult> Delete(int id)
        {
            var offer = await _context.Offers
                .Include(o => o.Highlights)
                .FirstOrDefaultAsync(o => o.OfferId == id);
            if (offer == null)
            {
                return NotFound();
            }
            return View(offer);
        }

        // Delete offer (POST)
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var offer = await _context.Offers
                .Include(o => o.Highlights)
                .FirstOrDefaultAsync(o => o.OfferId == id);
            if (offer != null)
            {
                _context.OfferHighlights.RemoveRange(offer.Highlights);
                _context.Offers.Remove(offer);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}