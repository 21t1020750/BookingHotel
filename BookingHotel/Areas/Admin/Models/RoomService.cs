namespace BookingHotel.Areas.Admin.Models
{
    public class RoomService
    {
        public int RoomID { get; set; }
        public int ServiceID { get; set; }

        // Navigation properties
        public Room Room { get; set; }
        public Service Service { get; set; }
    }
}