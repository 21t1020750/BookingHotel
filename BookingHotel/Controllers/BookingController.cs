using BookingHotel.Areas.Admin.Data;
using BookingHotel.Areas.Admin.Models;
using BookingHotel.Models;
using BookingHotel.Services;
using BookingHotel.Services.DataService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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

        public async Task<IActionResult> Index(List<string> selectedRoomIds, string checkin, string checkout, int adults, int children)
        {
            if (selectedRoomIds == null || !selectedRoomIds.Any() || string.IsNullOrEmpty(checkin) || string.IsNullOrEmpty(checkout))
            {
                return RedirectToAction("Index", "Find_Room");
            }

            var idsParam = string.Join(",", selectedRoomIds);
            var sql = $@"
                    SELECT r.*
                    FROM Rooms r
                    WHERE r.RoomID IN ({idsParam})
                ";

            var selectedRooms = await _db.Rooms
                .FromSqlRaw(sql)
                .Include(r => r.RoomServices)
                .ThenInclude(rs => rs.Service)
                .ToListAsync();

            if (selectedRooms == null || selectedRooms.Count == 0)
            {
                return NotFound();
            }

            var allServices = await _db.Services.ToListAsync();

            var viewModel = new BookingViewModel
            {
                RoomInfo = selectedRooms.Select(room => new RoomInfo
                {
                    RoomId = room.RoomID,
                    RoomNumber = room.RoomNumber,
                    Description = room.Description,
                    Price = room.Price,
                }).ToList(),
                RoomIds = selectedRooms.Select(r => r.RoomID).ToList(),
                Checkin = DateTime.Parse(checkin),
                Checkout = DateTime.Parse(checkout),
                Adults = adults,
                Children = children,
                Rooms = selectedRooms.Count,
                AllServices = allServices,
                TotalRoomPrice = selectedRooms.Sum(r => r.Price) * (DateTime.Parse(checkout) - DateTime.Parse(checkin)).Days
            };

            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Confirm(BookingViewModel model, List<string> selectedServiceIds)
        {
            model.SelectedServices = model.SelectedServices?.Where(s => s.Quantity > 0).ToList() ?? new List<ServiceInfo>();
            model.TotalServicePrice = 0;

            // Load thông tin phòng
            var parameterRooms = model.RoomIds.Select((id, i) => new SqlParameter($"@p{i}", id)).ToArray();
            var parameterRoomNames = string.Join(",", model.RoomIds.Select((_, i) => $"@p{i}"));
            var roomSql = $@"
                SELECT r.*
                FROM Rooms r
                WHERE r.RoomID IN ({parameterRoomNames})
            ";
            var selectedRooms = await _db.Rooms
                .FromSqlRaw(roomSql, parameterRooms)
                .Include(r => r.RoomServices)
                .ThenInclude(rs => rs.Service)
                .ToListAsync();

            model.RoomInfo = selectedRooms.Select(room => new RoomInfo
            {
                RoomId = room.RoomID,
                RoomNumber = room.RoomNumber ?? "Không xác định",
                Description = room.Description ?? "Không có mô tả",
                Price = room.Price,
            }).ToList();

            // Tính số đêm ở và tiền phòng
            int numberOfNights = Math.Max((model.Checkout - model.Checkin).Days, 1);
            model.TotalRoomPrice = model.RoomInfo.Sum(r => r.Price) * numberOfNights;

            // Load và cập nhật dịch vụ
            if (selectedServiceIds != null && selectedServiceIds.Any())
            {
                var serviceParams = selectedServiceIds.Select((id, i) => new SqlParameter($"@p{i}", id)).ToArray();
                var paramNames = string.Join(",", selectedServiceIds.Select((_, i) => $"@p{i}"));
                var serviceSql = $"SELECT * FROM Services WHERE ServiceID IN ({paramNames})";

                var services = await _db.Services
                    .FromSqlRaw(serviceSql, serviceParams)
                    .ToListAsync();

                // Kết hợp dịch vụ với số lượng từ model.SelectedServices
                var updatedSelectedServices = new List<ServiceInfo>();
                foreach (var service in services)
                {
                    var selectedService = model.SelectedServices?.FirstOrDefault(s => s.ServiceId == service.ServiceID);
                    int quantity = selectedService?.Quantity ?? 1; // Mặc định là 1 nếu không có số lượng
                    if (quantity > 0) // Chỉ thêm dịch vụ nếu số lượng > 0
                    {
                        updatedSelectedServices.Add(new ServiceInfo
                        {
                            ServiceId = service.ServiceID,
                            Name = service.ServiceName,
                            Price = service.Price,
                            Quantity = quantity
                        });
                    }
                }
                model.SelectedServices = updatedSelectedServices;
                model.TotalServicePrice = model.SelectedServices.Sum(s => s.Price * s.Quantity);
            }

            // Tổng tiền = tiền phòng + tiền dịch vụ
            decimal totalPrice = model.TotalRoomPrice + model.TotalServicePrice; // Không bao gồm VAT
            decimal vat = totalPrice * 0.12m; // VAT 12%
            decimal totalPriceWithVat = totalPrice + vat;

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState không hợp lệ: {Errors}", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                model.AllServices = await _db.Services.ToListAsync(); // Đảm bảo AllSoapServices được load lại
                return View("Index", model);
            }

            var customerId = GetCurrentCustomerId();
            if (customerId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var customer = await _db.Customers.FirstOrDefaultAsync(c => c.CustomerID == customerId.Value);
            if (customer == null || string.IsNullOrEmpty(customer.Email))
            {
                ModelState.AddModelError("", "Không tìm thấy email khách hàng.");
                model.AllServices = await _db.Services.ToListAsync();
                return View("Index", model);
            }

            // Tạo booking
            var booking = new Booking
            {
                CustomerID = customerId.Value,
                CheckInDate = model.Checkin,
                CheckOutDate = model.Checkout,
                CreatedAt = DateTime.Now,
                NumberOfRooms = model.Rooms,
                TotalPrice = totalPrice,
                BookingStatusID = 1,
                EmployeeID = null
            };
            _db.Bookings.Add(booking);
            await _db.SaveChangesAsync();

            string bookingCode = $"HT{DateTime.Now.Year}-{Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper()}";
            booking.BookingCode = bookingCode;
            await _db.SaveChangesAsync();
            model.BookingCode = bookingCode;

            // Lưu chi tiết phòng
            foreach (var roomId in model.RoomIds)
            {
                _db.BookingDetails.Add(new BookingDetail
                {
                    BookingID = booking.BookingID,
                    RoomID = roomId
                });
            }

            // Lưu dịch vụ
            foreach (var service in model.SelectedServices)
            {
                if (service.Quantity > 0) // Chỉ lưu dịch vụ có số lượng > 0
                {
                    _db.BookingServices.Add(new BookingService
                    {
                        BookingID = booking.BookingID,
                        ServiceID = service.ServiceId,
                        Quantity = service.Quantity,
                        TotalPrice = service.Price * service.Quantity
                    });
                }
            }

            await _db.SaveChangesAsync();

            // Gửi email
            try
            {
                var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "EmailTemplates", "BookingConfirmation.cshtml");
                var emailBody = await System.IO.File.ReadAllTextAsync(templatePath);
                var selectedRoomsText = string.Join("<br>", model.RoomInfo.Select(r =>
                    $"Phòng {r.RoomNumber} - {r.Price:N0} VND/đêm"));

                var selectedServicesText = model.SelectedServices != null && model.SelectedServices.Any()
                    ? string.Join("<br>", model.SelectedServices.Select(s =>
                        $"{s.Name ?? "Dịch vụ không xác định"} - {s.Quantity} x {s.Price:N0} VND"))
                    : "Không có dịch vụ nào";

                emailBody = emailBody
                    .Replace("{BookingCode}", booking.BookingCode)
                    .Replace("{SelectedRooms}", selectedRoomsText)
                    .Replace("{SelectedServices}", selectedServicesText)
                    .Replace("{Checkin}", model.Checkin.ToString("dd/MM/yyyy"))
                    .Replace("{Checkout}", model.Checkout.ToString("dd/MM/yyyy"))
                    .Replace("{NumberOfNights}", numberOfNights.ToString())
                    .Replace("{Rooms}", model.Rooms.ToString())
                    .Replace("{Adults}", model.Adults.ToString())
                    .Replace("{Children}", model.Children > 0 ? $"{model.Children} trẻ em" : "Không có")
                    .Replace("{TotalPrice}", totalPriceWithVat.ToString("N0"));

                await _emailService.SendEmailAsync(customer.Email, "Xác nhận đặt phòng - Lavida Hue Hotel", emailBody);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gửi email xác nhận cho {Email}", customer.Email);
                ModelState.AddModelError("", "Đặt phòng thành công, nhưng không thể gửi email xác nhận.");
            }

            model.IsBookingSuccessful = true;
            model.BookingID = booking.BookingID;

            _logger.LogInformation("IsBookingSuccessful set to true, BookingID: {BookingID}, RoomInfo count: {RoomCount}, SelectedServices: {ServiceCount}",
                model.BookingID, model.RoomInfo?.Count ?? 0, model.SelectedServices?.Count ?? 0);

            return View("Index", model);
        }


    }
}