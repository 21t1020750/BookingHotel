using BookingHotel.Areas.Admin.Data;
using BookingHotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BookingHotel.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ContentRoomsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ContentRoomsController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: /Admin/ContentRooms
        public async Task<IActionResult> Index()
        {
            var rooms = await _db.Content_Rooms.Include(r => r.RoomType).ToListAsync();
            return View(rooms);
        }

        // GET: /Admin/ContentRooms/Create
        public IActionResult Create()
        {
            ViewBag.title = "Thêm Ưu Đãi Mới";
            ViewBag.RoomTypes = new SelectList(_db.RoomTypes, "RoomTypeID", "TypeName");
            return View("Edit", new Content_Room());
        }

        // GET: /Admin/ContentAmenities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _db.Content_Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            ViewBag.RoomTypes = new SelectList(_db.RoomTypes, "RoomTypeID", "TypeName");
            ViewBag.title = "Sửa thông tin Tiện nghi";
            return View(room);
        }

        // POST: /Admin/ContentAmenities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, Content_Room room)
        {
            if (id != room.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (id == null) // Nếu không có id, tức là tạo mới
                {
                    _db.Add(room);
                }
                else // Nếu có id, tức là chỉnh sửa
                {
                    _db.Update(room);
                }

                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.RoomTypes = new SelectList(_db.RoomTypes, "RoomTypeID", "TypeName", "Description");
            return View(room);
        }

        // GET: /Admin/ContentAmenities/Delete/5
        public IActionResult Delete(int id)
        {
            var content_rooms = _db.Content_Rooms.Find(id);
            if (content_rooms != null)
            {
                _db.Content_Rooms.Remove(content_rooms);
                _db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}