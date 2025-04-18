namespace BookingHotel.Models
{
    public class Service
    {
        public int ServiceID { get; set; }
        public string ServiceName { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal Price { get; set; }
    }
}
