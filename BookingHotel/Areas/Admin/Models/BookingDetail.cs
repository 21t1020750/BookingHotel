namespace BookingHotel.Areas.Admin.Models
{
    public class BookingDetail
    {
        public int BookingDetailID { get; set; }
        public int BookingID { get; set; }
        public int RoomID { get; set; }

        public Booking Booking { get; set; }
        public Room Room { get; set; }
    }
}
