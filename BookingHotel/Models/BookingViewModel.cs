using BookingHotel.Areas.Admin.Models;

namespace BookingHotel.Models
{
    public class BookingViewModel
    {
        public int BookingID { get; set; }
        public int RoomId { get; set; }
        public string RoomNumber { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Photo { get; set; }
        public List<string> Services { get; set; }
        public DateTime Checkin { get; set; }
        public DateTime Checkout { get; set; }
        public int Adults { get; set; }
        public int Children { get; set; }
        public int Rooms { get; set; }
        public bool IsConfirmed { get; set; }
        public string PaymentMethod { get; set; }
        public List<int> RoomIds { get; set; }
        public int NumberOfRooms { get; set; }
        public int CustomerID { get; set; }
        public bool IsBookingSuccessful { get; set; }
        public string BookingCode { get; set; }
        public int RoomTypeID { get; set; }
        public List<RoomInfo> RoomInfo { get; set; }
        public List<Service> AllServices { get; set; }
        public List<ServiceInfo> SelectedServices { get; set; } = new List<ServiceInfo>();
        public decimal RoomTotal { get; set; }
        public decimal TotalRoomPrice { get; set; }
        public decimal TotalServicePrice { get; set; }
    }

    public class RoomInfo
    {
        public int RoomId { get; set; }
        public string RoomNumber { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountedPrice { get; set; } // Giá sau giảm
        public int? AppliedOfferId { get; set; } // ID ưu đãi
        public string AppliedOfferName { get; set; } // Tên ưu đãi
        public decimal DiscountPercentage { get; set; } // % giảm
    }

    public class ServiceInfo
    {
        public int ServiceId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}