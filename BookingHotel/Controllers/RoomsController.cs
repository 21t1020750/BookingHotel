using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingHotel.Areas.Admin.Models;
using System.Linq;
using System.Threading.Tasks;
using BookingHotel.Areas.Admin.Data;

namespace BookingHotel.Controllers
{
    public class RoomsController : Controller
    {
        private readonly ApplicationDbContext _context; // Thay YourDbContext bằng DbContext của bạn

        public RoomsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Lấy danh sách RoomType và các Room liên quan
            var roomTypes = await _context.RoomTypes
                .Include(rt => rt.Rooms)
                    .ThenInclude(r => r.RoomImages)
                .ToListAsync();

            return View(roomTypes);
        }
    }
}