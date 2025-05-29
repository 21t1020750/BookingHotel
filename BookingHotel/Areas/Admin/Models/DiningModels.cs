namespace BookingHotel.Areas.Admin.Models
{
    public class Restaurant
    {
        public int RestaurantID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string OperatingHours { get; set; }
        public string ImageURL { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class Tag
    {
        public int TagID { get; set; }
        public string TagName { get; set; }
    }

    public class RestaurantTag
    {
        public int RestaurantID { get; set; }
        public int TagID { get; set; }
        public Restaurant Restaurant { get; set; }
        public Tag Tag { get; set; }
    }

    public class Dish
    {
        public int DishID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Label { get; set; }
        public string ImageURL { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class HeroImage
    {
        public int HeroImageID { get; set; }
        public string SectionTitle { get; set; }
        public string Description { get; set; }
        public string ImageURL { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class DiningViewModel
    {
        public HeroImage HeroImage { get; set; }
        public List<RestaurantViewModel> Restaurants { get; set; }
        public List<Dish> Dishes { get; set; }
    }

    public class RestaurantViewModel
    {
        public Restaurant Restaurant { get; set; }
        public List<string> Tags { get; set; }
    }
}