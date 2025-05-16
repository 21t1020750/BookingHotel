using System.ComponentModel.DataAnnotations;

namespace BookingHotel.Areas.Admin.Models
{
    public class PaymentStatus
    {
        
        public int PaymentStatusID { get; set; }

       
        public string StatusName { get; set; }
    }
}