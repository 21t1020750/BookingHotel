namespace BookingHotel.Areas.Admin.Models
{
    public class BookingService
    {
        public int BookingServiceID { get; set; }
        public int BookingID { get; set; }
        public int ServiceID { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }

        public Booking Booking { get; set; }
        public Service Service { get; set; }
    }
}
