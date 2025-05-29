using Microsoft.AspNetCore.Mvc;
using BookingHotel.Areas.Admin.Data;
using BookingHotel.Areas.Admin.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingHotel.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DiningHeroImagesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DiningHeroImagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/DiningHeroImages
        public async Task<IActionResult> Index()
        {
            var heroImages = await _context.HeroImages.ToListAsync();
            return View(heroImages);
        }

        // GET: /Admin/DiningHeroImages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Admin/DiningHeroImages/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SectionTitle,Description,ImageURL")] HeroImage heroImage)
        {
            if (ModelState.IsValid)
            {
                heroImage.CreatedAt = DateTime.Now;
                _context.Add(heroImage);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Hình ảnh Hero đã được thêm thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(heroImage);
        }

        // GET: /Admin/DiningHeroImages/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var heroImage = await _context.HeroImages.FindAsync(id);
            if (heroImage == null)
            {
                return NotFound();
            }
            return View(heroImage);
        }

        // POST: /Admin/DiningHeroImages/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("HeroImageID,SectionTitle,Description,ImageURL,CreatedAt")] HeroImage heroImage)
        {
            if (id != heroImage.HeroImageID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(heroImage);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Hình ảnh Hero đã được cập nhật thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HeroImageExists(heroImage.HeroImageID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(heroImage);
        }

        // GET: /Admin/DiningHeroImages/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var heroImage = await _context.HeroImages.FindAsync(id);
            if (heroImage == null)
            {
                return NotFound();
            }
            return View(heroImage);
        }

        // POST: /Admin/DiningHeroImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var heroImage = await _context.HeroImages.FindAsync(id);
            if (heroImage != null)
            {
                _context.HeroImages.Remove(heroImage);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Hình ảnh Hero đã được xóa thành công!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool HeroImageExists(int id)
        {
            return _context.HeroImages.Any(e => e.HeroImageID == id);
        }
    }
}