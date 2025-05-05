using BookingHotel.Areas.Admin.Data;
using BookingHotel.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingHotel.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const string DefaultPhoto = "nophoto.png";

        public CustomersController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var customers = _context.Customers.ToList();
            return View(customers);
        }

        public IActionResult Create()
        {
            return View("Edit", new Customer { Photo = DefaultPhoto });
        }

        public IActionResult Edit(int? id)
        {
            Customer customer = id == null || id == 0
               ? new Customer { CustomerID = 0, Photo = DefaultPhoto }
               : _context.Customers.FirstOrDefault(e => e.CustomerID == id);

            if (customer == null) return NotFound();

            ViewBag.Title = id == 0 ? "Thêm khách hàng mới" : "Chỉnh sửa thông tin Khách Hàng";
            return View(customer);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Customer customer, IFormFile uploadPhoto)
        {

            if (_context.Customers.Any(r => r.FullName == customer.FullName && r.CustomerID != customer.CustomerID))
            {
                ModelState.AddModelError("FullName", "Tên Nhân Viên Đã tồn tại !");
            }

            if (_context.Customers.Any(r => r.Email == customer.Email && r.CustomerID != customer.CustomerID))
            {
                ModelState.AddModelError("Email", "Email Đã tồn tại !");
            }

            if (_context.Customers.Any(r => r.Phone == customer.Phone && r.CustomerID != customer.CustomerID))
            {
                ModelState.AddModelError("Phone", "Số điện thoại Đã tồn tại !");
            }

            if (ModelState.IsValid)
            {
                // Kiểm tra nếu không có ảnh tải lên
                if (uploadPhoto == null && customer.CustomerID == 0)
                {
                    customer.Photo = DefaultPhoto;
                }
                else if (uploadPhoto != null)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/Customer");

                    // Giữ nguyên tên file ảnh gốc
                    string fileName = Path.GetFileName(uploadPhoto.FileName);
                    string filePath = Path.Combine(uploadsFolder, fileName);

                    // Lưu ảnh vào thư mục
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await uploadPhoto.CopyToAsync(fileStream);
                    }

                    customer.Photo = fileName; // Lưu chỉ tên file, không lưu đường dẫn
                }

                if (customer.CustomerID == 0)
                {
                    customer.Password = "123456";
                    _context.Customers.Add(customer);
                }
                else
                {
                    _context.Customers.Update(customer);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(customer);
        }

        public IActionResult Delete(int id)
        {
            var customer = _context.Customers.Find(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
