using Microsoft.AspNetCore.Mvc;
using BookingHotel.Areas.Admin.Data;
using BookingHotel.Areas.Admin.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace BookingHotel.Controllers
{
    public class Find_RoomController : Controller
    {
        private readonly ApplicationDbContext _context;

        public Find_RoomController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(
            int page = 1,
            string sort = "recommended",
            decimal? maxPrice = null,
            int[] roomTypeIds = null,
            int[] serviceIds = null,
            DateTime? checkin = null,
            DateTime? checkout = null,
            int? adults = null,
            int? children = null)
        {
            int pageSize = 3;

            // Debug: Kiểm tra giá trị nhận được từ form
            Console.WriteLine($"Received: adults={adults}, children={children}, checkin={checkin}, checkout={checkout}");

            // Lấy danh sách phòng
            var roomsQuery = _context.Rooms
                .Include(r => r.RoomType)
                .Include(r => r.RoomStatus)
                .Include(r => r.RoomServices)
                .ThenInclude(rs => rs.Service)
                .AsQueryable();

            // Lọc theo giá
            if (maxPrice.HasValue)
                roomsQuery = roomsQuery.Where(r => r.Price <= maxPrice.Value);

            // Lọc theo loại chỗ nghỉ
            if (roomTypeIds != null && roomTypeIds.Length > 0)
                roomsQuery = roomsQuery.Where(r => roomTypeIds.Contains(r.RoomTypeID));

            // Lọc theo tiện nghi
            if (serviceIds != null && serviceIds.Length > 0)
                roomsQuery = roomsQuery.Where(r => r.RoomServices.Any(rs => serviceIds.Contains(rs.ServiceID)));

            // Lọc theo ngày và số người
            if (checkin.HasValue && checkout.HasValue && adults.HasValue && children.HasValue)
            {
                int totalGuests = adults.Value + children.Value;
                roomsQuery = roomsQuery.Where(r => r.number >= totalGuests);
                // Debug: Kiểm tra điều kiện lọc
                Console.WriteLine($"Filtering: number >= {totalGuests}");
            }
            else
            {
                Console.WriteLine("Filtering condition not met, no filtering by guests applied.");
            }

            // Sắp xếp
            switch (sort.ToLower())
            {
                case "price-low":
                    roomsQuery = roomsQuery.OrderBy(r => r.Price);
                    break;
                case "price-high":
                    roomsQuery = roomsQuery.OrderByDescending(r => r.Price);
                    break;
                case "recommended":
                default:
                    roomsQuery = roomsQuery.OrderBy(r => r.RoomID);
                    break;
            }

            var room = roomsQuery.ToList();

            // Phân trang
            int totalRooms = room.Count;
            int totalPages = (int)Math.Ceiling((double)totalRooms / pageSize);
            page = Math.Max(1, Math.Min(page, totalPages));
            var pagedRooms = room
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Lấy danh sách RoomType và Service
            var roomTypes = _context.RoomTypes.ToList();
            var services = _context.Services.ToList();
            ViewBag.RoomTypes = roomTypes;
            ViewBag.Amenities = services;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Sort = sort;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.RoomTypeIds = roomTypeIds?.ToList() ?? new List<int>();
            ViewBag.ServiceIds = serviceIds?.ToList() ?? new List<int>();
            ViewBag.Checkin = checkin;
            ViewBag.Checkout = checkout;
            ViewBag.Adults = adults;
            ViewBag.Children = children;

            return View(pagedRooms);
        }

    }
}