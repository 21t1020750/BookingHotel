using BookingHotel.Areas.Admin.Data;
using BookingHotel.Areas.Admin.Models;
using BookingHotel.Models;
using BookingHotel.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
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

            // Lấy ưu đãi hợp lệ
            var today = DateTime.Today;
            var checkinDate = DateTime.Parse(checkin);
            var offers = await _db.Offers
                .Include(o => o.Category)
                .Where(o => o.IsActive && o.ValidUntil >= today && o.Category.CategoryCode == "stay")
                .ToListAsync();

            // Áp dụng ưu đãi
            var roomInfoList = new List<RoomInfo>();
            foreach (var room in selectedRooms)
            {
                var applicableOffer = offers
                    .Where(o => o.ValidUntil >= checkinDate)
                    .OrderByDescending(o => o.DiscountPercentage)
                    .FirstOrDefault();

                decimal discountedPrice = applicableOffer != null
                    ? room.Price * (1 - (applicableOffer.DiscountPercentage / 100m))
                    : room.Price;

                roomInfoList.Add(new RoomInfo
                {
                    RoomId = room.RoomID,
                    RoomNumber = room.RoomNumber,
                    Description = room.Description,
                    Price = room.Price,
                    DiscountedPrice = discountedPrice,
                    AppliedOfferId = applicableOffer?.OfferId,
                    AppliedOfferName = applicableOffer?.Title,
                    DiscountPercentage = applicableOffer?.DiscountPercentage ?? 0
                });
            }

            var allServices = await _db.Services.ToListAsync();
            int numberOfNights = Math.Max((DateTime.Parse(checkout) - checkinDate).Days, 1);

            var viewModel = new BookingViewModel
            {
                RoomInfo = roomInfoList,
                RoomIds = selectedRooms.Select(r => r.RoomID).ToList(),
                Checkin = checkinDate,
                Checkout = DateTime.Parse(checkout),
                Adults = adults,
                Children = children,
                Rooms = selectedRooms.Count,
                NumberOfRooms = selectedRooms.Count,
                AllServices = allServices,
                TotalRoomPrice = roomInfoList.Sum(r => r.DiscountedPrice) * numberOfNights
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

            // Lấy ưu đãi hợp lệ
            var offers = await _db.Offers
                .Include(o => o.Category)
                .Where(o => o.IsActive && o.ValidUntil >= model.Checkin && o.Category.CategoryCode == "stay")
                .ToListAsync();

            // Áp dụng ưu đãi
            model.RoomInfo = selectedRooms.Select(room =>
            {
                var applicableOffer = offers
                    .Where(o => o.ValidUntil >= model.Checkin)
                    .OrderByDescending(o => o.DiscountPercentage)
                    .FirstOrDefault();

                decimal discountedPrice = applicableOffer != null
                    ? room.Price * (1 - (applicableOffer.DiscountPercentage / 100m))
                    : room.Price;

                return new RoomInfo
                {
                    RoomId = room.RoomID,
                    RoomNumber = room.RoomNumber ?? "Không xác định",
                    Description = room.Description ?? "Không có mô tả",
                    Price = room.Price,
                    DiscountedPrice = discountedPrice,
                    AppliedOfferId = applicableOffer?.OfferId,
                    AppliedOfferName = applicableOffer?.Title,
                    DiscountPercentage = applicableOffer?.DiscountPercentage ?? 0
                };
            }).ToList();

            // Tính số đêm và tiền phòng
            int numberOfNights = Math.Max((model.Checkout - model.Checkin).Days, 1);
            model.TotalRoomPrice = model.RoomInfo.Sum(r => r.DiscountedPrice) * numberOfNights;

            // Load và cập nhật dịch vụ
            if (selectedServiceIds != null && selectedServiceIds.Any())
            {
                var serviceParams = selectedServiceIds.Select((id, i) => new SqlParameter($"@p{i}", id)).ToArray();
                var paramNames = string.Join(",", selectedServiceIds.Select((_, i) => $"@p{i}"));
                var serviceSql = $"SELECT * FROM Services WHERE ServiceID IN ({paramNames})";

                var services = await _db.Services
                    .FromSqlRaw(serviceSql, serviceParams)
                    .ToListAsync();

                var updatedSelectedServices = new List<ServiceInfo>();
                foreach (var service in services)
                {
                    var selectedService = model.SelectedServices?.FirstOrDefault(s => s.ServiceId == service.ServiceID);
                    int quantity = selectedService?.Quantity ?? 1;
                    if (quantity > 0)
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

            // Tính tổng giá
            decimal totalPrice = model.TotalRoomPrice + model.TotalServicePrice;
            decimal vat = totalPrice * 0.12m;
            decimal totalPriceWithVat = totalPrice + vat;

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState không hợp lệ: {Errors}", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                model.AllServices = await _db.Services.ToListAsync();
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
                TotalPrice = totalPriceWithVat,
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
            foreach (var roomInfo in model.RoomInfo)
            {
                _db.BookingDetails.Add(new BookingDetail
                {
                    BookingID = booking.BookingID,
                    RoomID = roomInfo.RoomId,
                    DiscountedPrice = roomInfo.DiscountedPrice,
                    OfferId = roomInfo.AppliedOfferId
                });
            }

            // Lưu dịch vụ
            foreach (var service in model.SelectedServices)
            {
                if (service.Quantity > 0)
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
                    $"Phòng {r.RoomNumber} - Giá gốc: {r.Price:N0} VND/đêm" +
                    (r.AppliedOfferId.HasValue ? $"<br>Ưu đãi: {r.AppliedOfferName} ({r.DiscountPercentage}% giảm)<br>Giá sau giảm: {r.DiscountedPrice:N0} VND/đêm" : "")));

                var selectedServicesText = model.SelectedServices.Any()
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

            _logger.LogInformation("Đặt phòng thành công, BookingID: {BookingID}, Số phòng: {RoomCount}, Dịch vụ: {ServiceCount}",
                model.BookingID, model.RoomInfo?.Count ?? 0, model.SelectedServices?.Count ?? 0);

            return View("Index", model);
        }
    }
}