using BookingHotel.Areas.Admin.Data;
using BookingHotel.Areas.Admin.Models;
using BookingHotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Security.Claims;

namespace BookingHotel.Controllers
{
    public class InformationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const string DefaultPhoto = "nophoto.png";

        public InformationController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Account");
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == userEmail);

            if (customer == null)
            {
                return NotFound("Không tìm thấy thông tin khách hàng.");
            }

            // Lấy danh sách booking của khách hàng
            var bookings = await _context.Bookings
                .Where(b => b.CustomerID == customer.CustomerID)
                .Include(r => r.BookingStatus)// hoặc .Id tùy tên cột
                .OrderByDescending(b => b.CheckInDate)
                .ToListAsync();

            // Tạo ViewModel chứa cả thông tin khách và danh sách booking
            var model = new InformationViewModel
            {
                Customer = customer,
                Bookings = bookings
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(InformationViewModel model, IFormFile uploadPhoto)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == userEmail);

            if (customer == null)
            {
                return NotFound("Không tìm thấy khách hàng để cập nhật.");
            }

            // Cập nhật các trường thông tin
            customer.FullName = model.Customer.FullName;
            customer.Phone = model.Customer.Phone;
            customer.Address = model.Customer.Address;
            customer.DateOfBirth = model.Customer.DateOfBirth;

            // Xử lý ảnh đại diện
            if (uploadPhoto != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/Customer");
                string fileName = Path.GetFileName(uploadPhoto.FileName);
                string filePath = Path.Combine(uploadsFolder, fileName);

                // Lưu ảnh vào thư mục
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadPhoto.CopyToAsync(fileStream);
                }

                customer.Photo = fileName; // Cập nhật tên file ảnh vào Customer
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Thông tin đã được cập nhật thành công!";
            return RedirectToAction("Index");
        }
    }
}