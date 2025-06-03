using BookingHotel.Models;

namespace BookingHotel.Areas.Admin.Models
{
    public class RoomAmenitie
    {
        public int RoomAmenitieID { get; set; }
        public int RoomID { get; set; }
        public int AmenitieID { get; set; }

        public Room Room { get; set; }
        public Content_Amenity Amenities { get; set; }
    }
}
