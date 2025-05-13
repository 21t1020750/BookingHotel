using BookingHotel.Areas.Admin.Data;
using BookingHotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingHotel.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ContentOffersController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ContentOffersController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: /Admin/ContentOffers
        public async Task<IActionResult> Index()
        {
            var offers = await _db.Content_Offers.ToListAsync();
            return View(offers);
        }

        // GET: /Admin/ContentOffers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Admin/ContentOffers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Content_Offer offer)
        {
            if (ModelState.IsValid)
            {
                _db.Content_Offers.Add(offer);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(offer);
        }

        // GET: /Admin/ContentOffers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var offer = await _db.Content_Offers.FindAsync(id);
            if (offer == null)
            {
                return NotFound();
            }
            return View(offer);
        }

        // POST: /Admin/ContentOffers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Content_Offer offer)
        {
            if (id != offer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(offer);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OfferExists(offer.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(offer);
        }

        // GET: /Admin/ContentOffers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var offer = await _db.Content_Offers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (offer == null)
            {
                return NotFound();
            }

            return View(offer);
        }

        // POST: /Admin/ContentOffers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var offer = await _db.Content_Offers.FindAsync(id);
            if (offer != null)
            {
                _db.Content_Offers.Remove(offer);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool OfferExists(int id)
        {
            return _db.Content_Offers.Any(e => e.Id == id);
        }
    }
}