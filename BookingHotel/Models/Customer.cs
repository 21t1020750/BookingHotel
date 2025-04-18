namespace BookingHotel.Models
{
    public class Customer
    {
        public int CustomerID { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Address { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public string Photo { get; set; } = "";
    }
}
