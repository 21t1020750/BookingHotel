namespace BookingHotel.Models
{
    public class Review
    {
        public int ReviewID { get; set; }
        public string Title { get; set; } = "";
        public string Comment { get; set; } = "";   
        public int BookingID { get; set; }
        public int CustomerID { get; set; }
        public int Rating { get; set; }
        public DateTime ReviewDate { get; set; }
        public Booking Booking { get; set; }
        public Customer Customer { get; set; }  

    }
}
