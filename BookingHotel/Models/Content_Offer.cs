namespace BookingHotel.Models
{
    public class Content_Offer
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public DateTime ValidUntil { get; set; }
    }
}
