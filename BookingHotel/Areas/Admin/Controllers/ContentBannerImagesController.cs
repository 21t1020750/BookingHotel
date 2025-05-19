using BookingHotel.Areas.Admin.Data;
using BookingHotel.Areas.Admin.Models;
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
            return View("Edit", new Content_BannerImage());
        }

        // GET: /Admin/ContentBannerImages/Edit/5
        public IActionResult Edit(int? id)
        {
            Content_BannerImage content_BannerImage = id == null || id == 0
               ? new Content_BannerImage { Id = 0 }
               : _db.Content_BannerImages.FirstOrDefault(e => e.Id == id);

            if (content_BannerImage == null) return NotFound();

            ViewBag.Title = id == 0 ? "Thêm content Banner mới" : "Chỉnh sửa content Banner";
            return View(content_BannerImage);
        }

        // POST: /Admin/ContentBannerImages/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int? id, Content_BannerImage content_bannerImage)
        {
            if (id != content_bannerImage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (id == null) // Nếu không có id, tức là tạo mới
                {
                    _db.Add(content_bannerImage);
                }
                else // Nếu có id, tức là chỉnh sửa
                {
                    _db.Update(content_bannerImage);
                }

                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View("Edit", content_bannerImage);
        }

        public IActionResult Delete(int id)
        {
            var content_BannerImage = _db.Content_BannerImages.Find(id);
            if (content_BannerImage != null)
            {
                _db.Content_BannerImages.Remove(content_BannerImage);
                _db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}