using Microsoft.AspNetCore.Mvc;
using BookingHotel.Areas.Admin.Data;
using BookingHotel.Areas.Admin.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingHotel.Controllers
{
    public class Find_RoomController : Controller
    {
        private readonly ApplicationDbContext _db;

        public Find_RoomController(ApplicationDbContext db)
        {
            _db = db;
        }

        public class RoomViewModel
        {
            public Room Room { get; set; }
            public decimal DiscountedPrice { get; set; }
            public int? AppliedOfferId { get; set; }
        }

        public async Task<IActionResult> Index(
            string checkin, string checkout, int adults = 1, int children = 0, int rooms = 1,
            int page = 1, string sort = "recommended", decimal minPrice = 0, decimal maxPrice = 10000000,
            List<int> roomTypeIds = null, List<int> amenityIds = null, List<int> starRatings = null, int? offerId = null, string selectedRoomIds = null)
        {
            DateTime checkinDate;
            if (!DateTime.TryParse(checkin, out checkinDate))
            {
                checkinDate = DateTime.Today;
            }
            DateTime checkoutDate;
            if (!DateTime.TryParse(checkout, out checkoutDate))
            {
                checkoutDate = checkinDate.AddDays(1);
            }
            if (checkoutDate <= checkinDate)
            {
                checkoutDate = checkinDate.AddDays(1);
            }
            minPrice = Math.Max(0, minPrice);
            maxPrice = Math.Max(minPrice, maxPrice);
            int stayDuration = (checkoutDate - checkinDate).Days;
            var maxRoomPrice = await _db.Rooms.MaxAsync(r => r.Price);
            maxPrice = Math.Min(maxPrice, maxRoomPrice);
            var roomRatings = await _db.Reviews
                .Include(r => r.Booking)
                    .ThenInclude(b => b.BookingDetails)
                .SelectMany(r => r.Booking.BookingDetails, (r, bd) => new
                {
                    RoomID = bd.RoomID,
                    Rating = r.Rating
                })
                .GroupBy(x => x.RoomID)
                .Select(g => new
                {
                    RoomID = g.Key,
                    AvgRating = g.Average(x => x.Rating)
                })
                .ToListAsync();

            var roomsQuery = _db.Rooms
                .Include(r => r.RoomType)
                .Include(r => r.RoomStatus)
                .Include(r => r.RoomAmenities).ThenInclude(ra => ra.Amenities)
                .Include(r => r.RoomImages)
                .Where(r => r.IsDisplay)
                .AsQueryable();

            int totalGuests = adults + children;
            if (totalGuests > 0)
            {
                roomsQuery = roomsQuery.Where(r => r.number >= totalGuests);
            }

            if (checkinDate != DateTime.Today || checkoutDate != checkinDate.AddDays(1))
            {
                roomsQuery = roomsQuery.Where(r => !_db.BookingDetails
                    .Join(_db.Bookings,
                          bd => bd.BookingID,
                          b => b.BookingID,
                          (bd, b) => new { bd, b })
                    .Any(x => x.bd.RoomID == r.RoomID &&
                              x.b.BookingStatusID == 2 &&
                              !(checkoutDate <= x.b.CheckInDate || checkinDate >= x.b.CheckOutDate)));
            }

            roomsQuery = roomsQuery.Where(r => r.Price >= minPrice && r.Price <= maxPrice);

            if (roomTypeIds != null && roomTypeIds.Any())
            {
                roomsQuery = roomsQuery.Where(r => roomTypeIds.Contains(r.RoomTypeID));
            }

            if (amenityIds != null && amenityIds.Any())
            {
                roomsQuery = roomsQuery.Where(r => r.RoomAmenities.Any(ra => amenityIds.Contains(ra.AmenitieID)));
            }

            if (starRatings != null && starRatings.Any())
            {
                var validRoomIds = roomRatings
                    .Where(r =>
                    {
                        double avgRating = r.AvgRating;
                        return starRatings.Any(star =>
                            star == 0 && avgRating == 0 ||
                            star == 5 && avgRating >= 4.5 ||
                            star == 4 && avgRating >= 3.5 && avgRating < 4.5 ||
                            star == 3 && avgRating >= 2.5 && avgRating < 3.5 ||
                            star == 2 && avgRating >= 1.5 && avgRating < 2.5 ||
                            star == 1 && avgRating >= 1.0 && avgRating < 1.5);
                    })
                    .Select(r => r.RoomID)
                    .ToList();

                if (starRatings.Contains(0))
                {
                    var roomsWithNoRatings = await _db.Rooms
                        .Where(r => !roomRatings.Select(rr => rr.RoomID).Contains(r.RoomID))
                        .Select(r => r.RoomID)
                        .ToListAsync();
                    validRoomIds.AddRange(roomsWithNoRatings);
                }

                roomsQuery = roomsQuery.Where(r => validRoomIds.Contains(r.RoomID));
            }

            var offers = await _db.Offers
                .Include(o => o.Category)
                .Where(o => o.IsActive && o.ValidUntil >= checkinDate && o.Category.CategoryCode == "stay")
                .ToListAsync();

            var availableRooms = await roomsQuery.ToListAsync();

            var roomViewModels = availableRooms.Select(room =>
            {
                Offer applicableOffer = null;
                if (offerId.HasValue)
                {
                    var selectedOffer = offers.FirstOrDefault(o => o.OfferId == offerId.Value);
                    if (selectedOffer != null)
                    {
                        bool isEligible = true;
                        if (isEligible)
                        {
                            applicableOffer = selectedOffer;
                        }
                    }
                }
                else
                {
                    applicableOffer = offers.FirstOrDefault(o => o.ValidUntil >= checkinDate);
                }

                return new RoomViewModel
                {
                    Room = room,
                    DiscountedPrice = applicableOffer != null
                        ? room.Price * (1 - (applicableOffer.DiscountPercentage / 100m))
                        : room.Price,
                    AppliedOfferId = applicableOffer?.OfferId
                };
            }).ToList();

            switch (sort.ToLower())
            {
                case "price-low":
                    roomViewModels = roomViewModels.OrderBy(r => r.DiscountedPrice).ToList();
                    break;
                case "price-high":
                    roomViewModels = roomViewModels.OrderByDescending(r => r.DiscountedPrice).ToList();
                    break;
                case "recommended":
                default:
                    roomViewModels = roomViewModels.OrderBy(r => r.Room.RoomID).ToList();
                    break;
            }

            int pageSize = 4;
            int totalRooms = roomViewModels.Count;
            int totalPages = (int)Math.Ceiling((double)totalRooms / pageSize);
            page = Math.Max(1, Math.Min(page, totalPages));
            roomViewModels = roomViewModels
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.Checkin = checkinDate;
            ViewBag.Checkout = checkoutDate;
            ViewBag.Adults = adults;
            ViewBag.Children = children;
            ViewBag.Rooms = rooms;
            ViewBag.Sort = sort;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.RoomTypeIds = roomTypeIds ?? new List<int>();
            ViewBag.AmenityIds = amenityIds ?? new List<int>();
            ViewBag.StarRatings = starRatings ?? new List<int>();
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Offers = offers;
            ViewBag.SelectedOfferId = offerId;
            ViewBag.RoomTypes = await _db.RoomTypes.ToListAsync();
            ViewBag.Amenities = await _db.Content_Amenities.ToListAsync();
            ViewBag.RoomRatings = roomRatings.ToDictionary(r => r.RoomID, r => r.AvgRating);
            ViewBag.SelectedRoomIds = selectedRoomIds ?? "";

            return View(roomViewModels);
        }
    }
}
