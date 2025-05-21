using BookingHotel.Areas.Admin.Data;
using BookingHotel.Areas.Admin.Models;
using BookingHotel.Models;
using BookingHotel.Services;
using BookingHotel.Services.DataService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookingHotel.Controllers
{
    [Authorize(AuthenticationSchemes = "Cookies")]
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IEmailService _emailService;
        private readonly ILogger<BookingController> _logger;

        private int? GetCurrentCustomerId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : (int?)null;
        }

        public BookingController(ApplicationDbContext db, IEmailService emailService, ILogger<BookingController> logger)
        {
            _db = db;
            _emailService = emailService;
            _logger = logger;
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
                Rooms = rooms,
            };

            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Confirm(BookingViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model); // Trả lại form nếu dữ liệu không hợp lệ
            }

            var customerId = GetCurrentCustomerId(); // giả sử bạn có method để lấy UserID sau khi đăng nhập
            if (customerId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var customer = await _db.Customers.FirstOrDefaultAsync(c => c.CustomerID == customerId.Value);
            if (customer == null || string.IsNullOrEmpty(customer.Email))
            {
                ModelState.AddModelError("", "Không tìm thấy email khách hàng.");
                return View("Index", model);
            }

            // Tính tổng tiền = giá * số lượng phòng * số đêm
            int numberOfNights = (model.Checkout - model.Checkin).Days;
            decimal totalPrice = model.Price * model.Rooms * numberOfNights;


            // Tạo booking
            var booking = new Booking
            {
                CustomerID = customerId.Value,
                CheckInDate = model.Checkin,
                CheckOutDate = model.Checkout,
                CreatedAt = DateTime.Now,
                NumberOfRooms = model.Rooms,
                TotalPrice = totalPrice,
                BookingStatusID = 1, // Processing
                EmployeeID = null
            };

            _db.Bookings.Add(booking);
            await _db.SaveChangesAsync();


            // Trong action Confirm
            string bookingCode = $"HT{DateTime.Now.Year}-{Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper()}";
            booking.BookingCode = bookingCode;
            await _db.SaveChangesAsync();
            model.BookingCode = bookingCode;


            // Tạo chi tiết từng phòng
            for (int i = 0; i < model.Rooms; i++)
            {
                var bookingDetail = new BookingDetail
                {
                    BookingID = booking.BookingID,
                    RoomID = model.RoomId
                };
                _db.BookingDetails.Add(bookingDetail);
            }

            await _db.SaveChangesAsync();

            try
            {
                // Đọc template email
                var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "EmailTemplates", "BookingConfirmation.cshtml");
                var emailBody = await System.IO.File.ReadAllTextAsync(templatePath);

                // Thay thế placeholder
                emailBody = emailBody
                    .Replace("{BookingCode}", booking.BookingCode)
                    .Replace("{RoomNumber}", model.RoomNumber ?? "Không xác định")
                    .Replace("{Checkin}", model.Checkin.ToString("dd/MM/yyyy"))
                    .Replace("{Checkout}", model.Checkout.ToString("dd/MM/yyyy"))
                    .Replace("{NumberOfNights}", numberOfNights.ToString())
                    .Replace("{Rooms}", (model.Rooms).ToString())
                    .Replace("{Adults}", (model.Adults).ToString())
                    .Replace("{Children}", (model.Children) > 0 ? (model.Children).ToString() + " trẻ em" : "Không có")
                    .Replace("{TotalPrice}", (totalPrice * (1 + 0.12m)).ToString("N0"));

                await _emailService.SendEmailAsync(customer.Email, "Xác nhận đặt phòng - Lavela Hue Hotel", emailBody);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gửi email xác nhận cho {Email}", customer.Email);
                ModelState.AddModelError("", "Đặt phòng thành công, nhưng không thể gửi email xác nhận.");
            }

            model.IsBookingSuccessful = true;
            model.BookingID = booking.BookingID; // Lưu BookingID để hiển thị trong success section

            return View("Index", model);

        }
    }
}