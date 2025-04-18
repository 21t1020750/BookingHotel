using BookingHotel.Data;
using BookingHotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BookingHotel.Controllers.AdminController
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private void UpdateTotalPrice(int bookingID)
        {
            var booking = _context.Bookings
                .Include(b => b.BookingDetails).ThenInclude(d => d.Room)
                .Include(b => b.BookingServices)
                .FirstOrDefault(b => b.BookingID == bookingID);

            if (booking != null)
            {
                var roomTotal = booking.BookingDetails.Sum(d => d.Room.Price);
                var serviceTotal = booking.BookingServices.Sum(s => s.TotalPrice);

                booking.TotalPrice = roomTotal + serviceTotal;

                _context.SaveChanges();
            }
        }

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var bookings = _context.Bookings
                .Include(r => r.Customer)
                .Include(r => r.Employee)
                .Include(r => r.BookingStatus)
                .ToList();
            return View(bookings);
        }

        public IActionResult Details(int id)
        {
            var booking = _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Employee)
                .Include(b => b.BookingStatus)
                .Include(b => b.BookingDetails)
                    .ThenInclude(bd => bd.Room)  
                .Include(b => b.BookingServices)
                    .ThenInclude(bs => bs.Service)
                .FirstOrDefault(b => b.BookingID == id);

            if (booking == null)
            {
                return NotFound(); 
            }
            UpdateTotalPrice(id);
            return View(booking); 
        }

        [HttpGet]
        public IActionResult Accept(int id)
        {
            var booking = _context.Bookings.Find(id);
            if (booking == null)
                return NotFound();

           
            booking.BookingStatusID = 2;
            _context.SaveChanges();

            
            return RedirectToAction("Details", new { id });
        }

        [HttpGet]
        public IActionResult Cancel(int id)
        {
            var booking = _context.Bookings.Find(id);
            if (booking == null)
                return NotFound();

            // Giả sử BookingStatusID = 3 là "Cancel"
            booking.BookingStatusID = 3;
            _context.SaveChanges();

            
            return RedirectToAction("Details", new { id });
        }

        public IActionResult DeleteRoom(int bookingID, int bookingDetailID)
        {
            var bookingDetail = _context.BookingDetails
                .Include(d => d.Room)
                .FirstOrDefault(d => d.BookingDetailID == bookingDetailID && d.BookingID == bookingID);

            if (bookingDetail != null)
            {
                _context.BookingDetails.Remove(bookingDetail);
                _context.SaveChanges();

                var booking = _context.Bookings.FirstOrDefault(b => b.BookingID == bookingID);
                if (booking != null)
                {
                    booking.NumberOfRooms = _context.BookingDetails.Count(d => d.BookingID == bookingID);
                    UpdateTotalPrice(bookingID);
                    _context.SaveChanges();
                }
            }

            // Sau khi xóa, quay lại trang cập nhật booking
            return RedirectToAction("Details", new { id = bookingID });
        }

        public IActionResult DeleteService(int bookingID, int bookingServiceID)
        {
            var bookingService = _context.BookingServices
                .Include(s => s.Service)
                .FirstOrDefault(s => s.BookingServiceID == bookingServiceID && s.BookingID == bookingID);

            if (bookingService != null)
            {
                _context.BookingServices.Remove(bookingService);
                _context.SaveChanges();

                UpdateTotalPrice(bookingID);
            }

            return RedirectToAction("Details", new { id = bookingID });
        }

        [HttpGet]
        public IActionResult UpdateService(int bookingID, int bookingServiceID)
        {
            var bookingService = _context.BookingServices
                .Include(s => s.Service)
                .FirstOrDefault(s => s.BookingServiceID == bookingServiceID && s.BookingID == bookingID);

            if (bookingService == null)
            {
                return NotFound();
            }

            return PartialView("EditService", bookingService); // hoặc View nếu bạn muốn
        }

        [HttpPost]
        public IActionResult UpdateService(BookingService model, int quantity, int bookingID)
        {
            var bookingService = _context.BookingServices
                .Include(bs => bs.Service)
                .FirstOrDefault(bs => bs.BookingServiceID == model.BookingServiceID);

            if (bookingService == null)
            {
                return NotFound();
            }

            // Cập nhật số lượng và giá
            bookingService.Quantity = quantity;
            bookingService.TotalPrice = quantity * bookingService.Service.Price;

            _context.SaveChanges();

            // Sau khi cập nhật, quay lại chi tiết booking
            return RedirectToAction("Details", new { id = bookingID });
        }

        public IActionResult Create()
        {
            ViewBag.Customers = new SelectList(_context.Customers, "CustomerID", "FullName");
            return View();
        }

    }
}
