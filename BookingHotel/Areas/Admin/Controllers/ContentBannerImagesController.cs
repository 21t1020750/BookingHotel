using BookingHotel.Areas.Admin.Data;
using BookingHotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingHotel.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ContentBannerImagesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ContentBannerImagesController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: /Admin/ContentBannerImages
        public async Task<IActionResult> Index()
        {
            var bannerImages = await _db.Content_BannerImages.ToListAsync();
            return View(bannerImages);
        }

        // GET: /Admin/ContentBannerImages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Admin/ContentBannerImages/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Content_BannerImage bannerImage)
        {
            if (ModelState.IsValid)
            {
                _db.Content_BannerImages.Add(bannerImage);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(bannerImage);
        }

        // GET: /Admin/ContentBannerImages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bannerImage = await _db.Content_BannerImages.FindAsync(id);
            if (bannerImage == null)
            {
                return NotFound();
            }
            return View(bannerImage);
        }

        // POST: /Admin/ContentBannerImages/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Content_BannerImage bannerImage)
        {
            if (id != bannerImage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(bannerImage);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BannerImageExists(bannerImage.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(bannerImage);
        }

        // GET: /Admin/ContentBannerImages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bannerImage = await _db.Content_BannerImages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bannerImage == null)
            {
                return NotFound();
            }

            return View(bannerImage);
        }

        // POST: /Admin/ContentBannerImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bannerImage = await _db.Content_BannerImages.FindAsync(id);
            if (bannerImage != null)
            {
                _db.Content_BannerImages.Remove(bannerImage);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool BannerImageExists(int id)
        {
            return _db.Content_BannerImages.Any(e => e.Id == id);
        }
    }
}