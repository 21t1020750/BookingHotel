using BookingHotel.Areas.Admin.Data;
using BookingHotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingHotel.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ContentAchivementsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ContentAchivementsController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: /Admin/Contentachivements
        public async Task<IActionResult> Index()
        {
            var achivements = await _db.Content_Achivements.ToListAsync();
            return View(achivements);
        }

        // GET: /Admin/Contentachivements/Create
        public IActionResult Create()
        {
            ViewBag.title = "Thêm Thành Tựu mới";
            return View("Edit", new Content_Achivement());
        }

        // GET: /Admin/ContentAmenities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var achivement = await _db.Content_Achivements.FindAsync(id);
            if (achivement == null)
            {
                return NotFound();
            }
            ViewBag.title = "Sửa nội dung Thành Tựu";
            return View(achivement);
        }

        // POST: /Admin/ContentAmenities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, Content_Achivement achivement)
        {
            if (id != achivement.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (id == null) // Nếu không có id, tức là tạo mới
                {
                    _db.Add(achivement);
                }
                else // Nếu có id, tức là chỉnh sửa
                {
                    _db.Update(achivement);
                }

                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(achivement);
        }

        // GET: /Admin/ContentAmenities/Delete/5
        public IActionResult Delete(int id)
        {
            var achivements = _db.Content_Achivements.Find(id);
            if (achivements != null)
            {
                _db.Content_Achivements.Remove(achivements);
                _db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
