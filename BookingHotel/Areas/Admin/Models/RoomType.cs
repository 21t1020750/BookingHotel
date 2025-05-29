using System.ComponentModel.DataAnnotations;
using System.Collections.Generic; // Add this for List<T>

namespace BookingHotel.Areas.Admin.Models
{
    public class RoomType
    {
        [Key]
        public int RoomTypeID { get; set; }
        public string TypeName { get; set; } = "";
        public string Description { get; set; } = "";

        // Navigation property for related Rooms
        public List<Room> Rooms { get; set; } = new List<Room>();
    }
}