using BookingHotel.Areas.Admin.Data;
using BookingHotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingHotel.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _db;

        public BookingController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(int roomId, string checkin, string checkout, int adults, int children, int rooms)
        {
            // Kiểm tra dữ liệu đầu vào
            if (roomId <= 0 || string.IsNullOrEmpty(checkin) || string.IsNullOrEmpty(checkout))
            {
                return RedirectToAction("Index", "Find_Room"); // Chuyển về trang tìm kiếm nếu dữ liệu không hợp lệ
            }

            // Truy vấn thông tin phòng từ cơ sở dữ liệu
            var room = await _db.Rooms
                .Include(r => r.RoomServices)
                .ThenInclude(rs => rs.Service)
                .FirstOrDefaultAsync(r => r.RoomID == roomId);

            if (room == null)
            {
                return NotFound(); // Trả về lỗi 404 nếu không tìm thấy phòng
            }

            // Tạo ViewModel
            var viewModel = new BookingViewModel
            {
                RoomId = room.RoomID,
                RoomNumber = room.RoomNumber,
                Description = room.Description,
                Price = room.Price,
                Photo = room.Photo,
                Services = room.RoomServices?.Select(rs => rs.Service.ServiceName).ToList() ?? new List<string>(),
                Checkin = DateTime.Parse(checkin),
                Checkout = DateTime.Parse(checkout),
                Adults = adults,
                Children = children,
                Rooms = rooms
            };

            return View(viewModel);
        }
    }
}