namespace BookingHotel.Models
{
    public class HomeViewModel
    {
        public List<Content_BannerImage> BannerImages { get; set; }
        public List<Content_Achivement> Achivements { get; set; }
        public List<Content_Room> Rooms { get; set; }
        public List<Content_Amenity> Amenities { get; set; }
        public List<Content_Offer> Offers { get; set; }
        public List<Content_MembershipBenefit> MembershipBenefits { get; set; }
    }
}
