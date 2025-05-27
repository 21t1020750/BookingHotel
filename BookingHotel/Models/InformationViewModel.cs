using BookingHotel.Areas.Admin.Models;

namespace BookingHotel.Models
{
    public class InformationViewModel
    {
        public Customer Customer { get; set; }
        public List<Booking> Bookings { get; set; }
        public BookingStatus BookingStatus { get; set; }
    }
}
