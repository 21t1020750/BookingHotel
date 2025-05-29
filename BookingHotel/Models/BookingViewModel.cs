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
        public List<string> Services { get; set; } // Danh sách tiện nghi
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

        // Tổng tiền phòng, dịch vụ, tổng thanh toán
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
    }

    public class ServiceInfo
    {
        public int ServiceId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }  // Nếu bạn cần số lượng dịch vụ
    }
}