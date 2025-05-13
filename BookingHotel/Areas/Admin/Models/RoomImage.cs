using System.ComponentModel.DataAnnotations;

namespace BookingHotel.Areas.Admin.Models
{
    public class RoomImage
    {
        [Key]
        public int RoomImageID { get; set; }
        public int RoomID { get; set; }
        public string ImagePath { get; set; } = "";
        public int DisplayOrder { get; set; }
        public Room Room { get; set; }
    }
}
