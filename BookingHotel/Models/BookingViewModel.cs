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

    }
}