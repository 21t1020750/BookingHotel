using BookingHotel.Data;
using BookingHotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BookingHotel.Controllers.AdminController
{
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const string DefaultPhoto = "nophoto.png";

        public EmployeesController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var employees = _context.Employees.ToList();
            return View(employees);
        }

        public IActionResult Create()
        {
            return View("Edit", new Employee { Photo = DefaultPhoto });
        }

        public IActionResult Edit(int? id)
        {
            Employee employee = id == null || id == 0
                 ? new Employee { EmployeeID = 0, Photo = DefaultPhoto }
                 : _context.Employees.FirstOrDefault(e => e.EmployeeID == id);

            if (employee == null) return NotFound();

            ViewBag.Title = id == 0 ? "Thêm nhân viên mới" : "Chỉnh sửa nhân viên";
            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Employee employee, IFormFile uploadPhoto)
        {

            if (_context.Employees.Any(r => r.FullName == employee.FullName && r.EmployeeID != employee.EmployeeID))
            {
                ModelState.AddModelError("FullName", "Tên Nhân Viên Đã tồn tại !");
            }

            if (_context.Employees.Any(r => r.Email == employee.Email && r.EmployeeID != employee.EmployeeID))
            {
                ModelState.AddModelError("Email", "Email Đã tồn tại !");
            }

            if (_context.Employees.Any(r => r.Phone == employee.Phone && r.EmployeeID != employee.EmployeeID))
            {
                ModelState.AddModelError("Phone", "Số điện thoại Đã tồn tại !");
            }

            if (ModelState.IsValid)
            {
                // Kiểm tra nếu không có ảnh tải lên
                if (uploadPhoto == null && employee.EmployeeID == 0)
                {
                    employee.Photo = DefaultPhoto;
                }
                else if (uploadPhoto != null)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/Employee");

                    // Giữ nguyên tên file ảnh gốc
                    string fileName = Path.GetFileName(uploadPhoto.FileName);
                    string filePath = Path.Combine(uploadsFolder, fileName);

                    // Lưu ảnh vào thư mục
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await uploadPhoto.CopyToAsync(fileStream);
                    }

                    employee.Photo = fileName; // Lưu chỉ tên file, không lưu đường dẫn
                }

                if (employee.EmployeeID == 0)
                {
                    _context.Employees.Add(employee);
                }
                else
                {
                    _context.Employees.Update(employee);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(employee);
        }

        public IActionResult Delete(int id)
        {
            var employee = _context.Employees.Find(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
