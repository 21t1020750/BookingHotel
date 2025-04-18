using System.ComponentModel.DataAnnotations;

namespace BookingHotel.Models
{
    public class RoomType
    {
        [Key] 
        public int RoomTypeID { get; set; }
        public string TypeName { get; set; } = "";

        public string Description { get; set; } = "";
    }
}
