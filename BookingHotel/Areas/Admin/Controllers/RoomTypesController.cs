using BookingHotel.Areas.Admin.Data;
using BookingHotel.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingHotel.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RoomTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoomTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách RoomTypes
        public IActionResult Index()
        {
            var roomTypes = _context.RoomTypes.ToList();
            return View(roomTypes);
        }

        // GET: Create or Edit
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung phòng";
            return View("Edit", new RoomType());
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roomType = _context.RoomTypes.Find(id);
            if (roomType == null)
            {
                return NotFound();
            }

            ViewBag.Title = "Chỉnh sửa phòng";
            return View("Edit", roomType);
        }


        [HttpPost]
        public IActionResult Edit(int? id, RoomType roomType)
        {
            if (id != null && id != roomType.RoomTypeID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (id == null) // Nếu không có id, tức là tạo mới
                {
                    _context.Add(roomType);
                }
                else // Nếu có id, tức là chỉnh sửa
                {
                    _context.Update(roomType);
                }

                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View("Edit", roomType); // Trả về lại view với dữ liệu hiện tại nếu có lỗi
        }

        // Xóa phòng
        public IActionResult Delete(int id)
        {
            var room = _context.RoomTypes.Find(id);
            if (room != null)
            {
                _context.RoomTypes.Remove(room);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
    