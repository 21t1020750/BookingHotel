using BookingHotel.Models;

namespace BookingHotel.Areas.Admin.Models
{
    public class Booking
    {
        public int BookingID { get; set; }
        public int CustomerID { get; set; }
        public int BookingStatusID { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public int EmployeeID { get; set; }
        public int NumberOfRooms { get; set; }
        public decimal TotalPrice { get; set; }

        public ICollection<BookingDetail> BookingDetails { get; set; }
        public ICollection<BookingService> BookingServices { get; set; }
        public Customer Customer { get; set; } // Quan hệ với RoomType
        public Employee Employee { get; set; }
        public BookingStatus BookingStatus { get; set; }

    }
}
