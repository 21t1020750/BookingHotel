using BookingHotel.Areas.Admin.Data;
using BookingHotel.Models;
using Microsoft.AspNetCore.Mvc;
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
            var rooms = await _db.Content_Rooms.ToListAsync();
            return View(rooms);
        }

        // GET: /Admin/ContentRooms/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Admin/ContentRooms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Content_Room room)
        {
            if (ModelState.IsValid)
            {
                _db.Content_Rooms.Add(room);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(room);
        }

        // GET: /Admin/ContentRooms/Edit/5
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
            return View(room);
        }

        // POST: /Admin/ContentRooms/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Content_Room room)
        {
            if (id != room.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(room);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomExists(room.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(room);
        }

        // GET: /Admin/ContentRooms/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _db.Content_Rooms
                .FirstOrDefaultAsync(m => m.Id == id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        // POST: /Admin/ContentRooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _db.Content_Rooms.FindAsync(id);
            if (room != null)
            {
                _db.Content_Rooms.Remove(room);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool RoomExists(int id)
        {
            return _db.Content_Rooms.Any(e => e.Id == id);
        }
    }
}