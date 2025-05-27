namespace BookingHotel.Areas.Admin.Services
{
    public interface IEmailServiceAdmin
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
