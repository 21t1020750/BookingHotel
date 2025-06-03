namespace BookingHotel.Areas.Admin.Models
{
    public class BookingDetail
    {
        public int BookingDetailID { get; set; }
        public int BookingID { get; set; }
        public int RoomID { get; set; }
        public Booking Booking { get; set; }
        public Room Room { get; set; }
        public decimal DiscountedPrice { get; set; } // Giá sau giảm
        public int? OfferId { get; set; } // ID ưu đãi áp dụng
        public Offer Offer { get; set; } // Liên kết đến Offer
    }
}