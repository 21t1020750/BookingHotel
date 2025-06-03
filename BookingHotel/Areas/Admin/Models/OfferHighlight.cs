using System.ComponentModel.DataAnnotations;

namespace BookingHotel.Areas.Admin.Models
{
    public class OfferHighlight
    {
        [Key]
        public int HighlightId { get; set; }
        public int OfferId { get; set; }
        public string HighlightText { get; set; }
        public string IconClass { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Offer Offer { get; set; }
    }
}