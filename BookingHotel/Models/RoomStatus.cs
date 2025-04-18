using System.ComponentModel.DataAnnotations;

namespace BookingHotel.Models
{
    public class RoomStatus
    {
        [Key]
        public int StatusID { get; set; }
        public string Description { get; set; } = "";
    }
}
