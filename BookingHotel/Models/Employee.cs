namespace BookingHotel.Models
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        public string FullName { get; set; } = "";
        public string Position { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public decimal Salary { get; set; }
        public DateTime HireDate { get; set; }
        public string Photo { get; set; } = "";
        public string Password { get; set; } = "";
    }
}
