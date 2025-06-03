using System.ComponentModel.DataAnnotations;

namespace BookingHotel.Models
{
    public class ContactFormModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập chủ đề")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung")]
        public string Message { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Phone { get; set; }
    }
}
