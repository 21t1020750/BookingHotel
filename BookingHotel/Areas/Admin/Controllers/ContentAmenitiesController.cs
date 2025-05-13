using BookingHotel.Areas.Admin.Data;
using BookingHotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingHotel.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ContentAmenitiesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ContentAmenitiesController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: /Admin/ContentAmenities
        public async Task<IActionResult> Index()
        {
            var amenities = await _db.Content_Amenities.ToListAsync();
            return View(amenities);
        }

        // GET: /Admin/ContentAmenities/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Admin/ContentAmenities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Content_Amenity amenity)
        {
            if (ModelState.IsValid)
            {
                _db.Content_Amenities.Add(amenity);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(amenity);
        }

        // GET: /Admin/ContentAmenities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var amenity = await _db.Content_Amenities.FindAsync(id);
            if (amenity == null)
            {
                return NotFound();
            }
            return View(amenity);
        }

        // POST: /Admin/ContentAmenities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Content_Amenity amenity)
        {
            if (id != amenity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(amenity);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AmenityExists(amenity.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(amenity);
        }

        // GET: /Admin/ContentAmenities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var amenity = await _db.Content_Amenities
                .FirstOrDefaultAsync(m => m.Id == id);
            if (amenity == null)
            {
                return NotFound();
            }

            return View(amenity);
        }

        // POST: /Admin/ContentAmenities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var amenity = await _db.Content_Amenities.FindAsync(id);
            if (amenity != null)
            {
                _db.Content_Amenities.Remove(amenity);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool AmenityExists(int id)
        {
            return _db.Content_Amenities.Any(e => e.Id == id);
        }
    }
}