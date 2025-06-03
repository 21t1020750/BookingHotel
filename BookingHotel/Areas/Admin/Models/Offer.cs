namespace BookingHotel.Areas.Admin.Models
{
    public class Offer
    {
        public int OfferId { get; set; }
        public int CategoryId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int DiscountPercentage { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal DiscountedPrice { get; set; }
        public string PriceUnit { get; set; }
        public DateTime ValidUntil { get; set; }
        public string IconClass { get; set; }
        public string AltText { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; }

        public Category Category { get; set; }
        public List<OfferHighlight> Highlights { get; set; }
    }
}