using BookingHotel.Areas.Admin.Models;

namespace BookingHotel.Models
{
    public class FindRoomViewModel
    {
        public List<Room> Rooms { get; set; }
        public List<RoomType> RoomTypes { get; set; }
        public List<Service> Services { get; set; }
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public int Adults { get; set; }
        public int Children { get; set; }
        public int Room { get; set; }
    }
}