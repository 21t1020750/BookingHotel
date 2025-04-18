namespace BookingHotel.Models
{
    public class Room
    {
        public int RoomID { get; set; }
        public string RoomNumber { get; set; } = "";
        public decimal Price { get; set;}
        public string Description { get; set; } = "";
        public string Photo { get; set; } = "";
        public int RoomTypeID { get; set; }
        public int StatusID { get; set; }
        public RoomType RoomType { get; set; } // Quan hệ với RoomType
        public RoomStatus RoomStatus { get; set; }
    }
}
