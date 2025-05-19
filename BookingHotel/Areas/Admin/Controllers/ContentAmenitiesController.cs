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

       
        public IActionResult Create()
        {
            ViewBag.title = "Thêm Tiện nghi mới";
            return View("Edit", new Content_Amenity());
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
            ViewBag.title = "Sửa thông tin Tiện nghi";
            return View(amenity);
        }

        // POST: /Admin/ContentAmenities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, Content_Amenity amenity)
        {
            if (id != amenity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (id == null) // Nếu không có id, tức là tạo mới
                {
                    _db.Add(amenity);
                }
                else // Nếu có id, tức là chỉnh sửa
                {
                    _db.Update(amenity);
                }

                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(amenity);
        }

        // GET: /Admin/ContentAmenities/Delete/5
        public IActionResult Delete(int id)
        {
            var content_Amenities = _db.Content_Amenities.Find(id);
            if (content_Amenities != null)
            {
                _db.Content_Amenities.Remove(content_Amenities);
                _db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}