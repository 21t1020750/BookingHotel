using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace BookingHotel.Areas.Admin.Models
{
    public class Room
    {
        // Thuộc tính cho ưu đãi
        [NotMapped]
        public decimal DiscountedPrice { get; set; }
        [NotMapped]
        public int? AppliedOfferId { get; set; }
        public int RoomID { get; set; }
        public string RoomNumber { get; set; } = "";
        public decimal Price { get; set; }
        public string Description { get; set; } = "";
        public string Photo { get; set; } = "";
        public int RoomTypeID { get; set; }
        public int StatusID { get; set; }
        public RoomType RoomType { get; set; } // Quan hệ với RoomType
        public RoomStatus RoomStatus { get; set; }
        public int number { get; set; }
        public List<RoomImage> RoomImages { get; set; }
        [NotMapped]
        public List<IFormFile> NewImages { get; set; }
        public List<RoomService> RoomServices { get; set; } = new List<RoomService>();
        // Add this property to fix the error
        public List<RoomAmenitie> RoomAmenities { get; set; }
        public bool IsDisplay { get; set; }
    }
}