using Microsoft.AspNetCore.Mvc;
using BookingHotel.Areas.Admin.Data;
using BookingHotel.Areas.Admin.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingHotel.Controllers
{
    public class Find_RoomController : Controller
    {
        private readonly ApplicationDbContext _context;

        public Find_RoomController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Lấy danh sách phòng
            var rooms = _context.Rooms
                .Include(r => r.RoomType)
                .Include(r => r.RoomStatus)
                .ToList();

            // Lấy danh sách RoomType từ cơ sở dữ liệu
            var roomTypes = _context.RoomTypes.ToList();
            ViewBag.RoomTypes = roomTypes;

            // Lấy danh sách Service từ cơ sở dữ liệu
            var services = _context.Services.ToList();
            ViewBag.Amenities = services;

            return View(rooms);
        }
    }
}