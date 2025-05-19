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
            ViewBag.title = "Thêm Ưu Đãi Mới";
            return View("Edit", new Content_Offer());
        }

        // GET: /Admin/ContentAmenities/Edit/5
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
            ViewBag.title = "Sửa thông tin Tiện nghi";
            return View(offer);
        }

        // POST: /Admin/ContentAmenities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, Content_Offer offer)
        {
            if (id != offer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (id == null) // Nếu không có id, tức là tạo mới
                {
                    _db.Add(offer);
                }
                else // Nếu có id, tức là chỉnh sửa
                {
                    _db.Update(offer);
                }

                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(offer);
        }

        // GET: /Admin/ContentAmenities/Delete/5
        public IActionResult Delete(int id)
        {
            var content_Offers = _db.Content_Offers.Find(id);
            if (content_Offers != null)
            {
                _db.Content_Offers.Remove(content_Offers);
                _db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}