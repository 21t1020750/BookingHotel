using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingHotel.Areas.Admin.Models
{
    public class Payment
    {
        public int PaymentID { get; set; }
        public int BookingID { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } // Ví dụ: "Credit Card", "Cash"
        public int PaymentStatusID { get; set; } // Thêm cột này
        public DateTime PaymentDate { get; set; }

        public virtual Booking Booking { get; set; }
        public virtual PaymentStatus PaymentStatus { get; set; } // Quan hệ với PaymentStatus
    }
}