using BookingHotel.Areas.Admin.Models;

namespace BookingHotel.Models
{
    public class Content_Room
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public int RoomTypeID { get; set; }
        public RoomType RoomType { get; set; }
    }
}
