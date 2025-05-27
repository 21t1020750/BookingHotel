using BookingHotel.Areas.Admin.Data;
using BookingHotel.Areas.Admin.Models;
using BookingHotel.Areas.Admin.Services;
using BookingHotel.Controllers;
using BookingHotel.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BookingHotel.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const string DefaultPhoto = "nophoto.png";
        private readonly IEmailServiceAdmin _emailServiceAdmin;
        private readonly ILogger<EmployeesController> _logger;
        public string GenerateRandomPassword(int length = 8)
        {
            const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*?_-";
            var random = new Random();
            return new string(Enumerable.Repeat(validChars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public EmployeesController(ApplicationDbContext context, IEmailServiceAdmin emailServiceAdmin, ILogger<EmployeesController> logger)
        {
            _context = context;
            _emailServiceAdmin = emailServiceAdmin;
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
                if (employee.EmployeeID == 0)
                {
                    // Tạo mới nhân viên
                    if (uploadPhoto == null)
                    {
                        employee.Photo = DefaultPhoto;
                    }
                    else
                    {
                        string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/Employee");
                        string fileName = Path.GetFileName(uploadPhoto.FileName);
                        string filePath = Path.Combine(uploadsFolder, fileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await uploadPhoto.CopyToAsync(fileStream);
                        }
                        employee.Photo = fileName;
                    }

                    var randomPassword = GenerateRandomPassword();
                    employee.Password = randomPassword;

                    _context.Employees.Add(employee);

                    // Gửi email mật khẩu
                    var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "EmailTemplates", "PasswordConfirmation.cshtml");
                    var emailBody = await System.IO.File.ReadAllTextAsync(templatePath);
                    emailBody = emailBody.Replace("{Email}", employee.Email)
                                         .Replace("{Password}", employee.Password);

                    await _emailServiceAdmin.SendEmailAsync(employee.Email, "Xác nhận mật khẩu", emailBody);
                }
                else
                {
                    // Cập nhật nhân viên
                    var existingEmployee = await _context.Employees.FindAsync(employee.EmployeeID);
                    if (existingEmployee == null) return NotFound();

                    existingEmployee.FullName = employee.FullName;
                    existingEmployee.Email = employee.Email;
                    existingEmployee.Phone = employee.Phone;
                    existingEmployee.Roles = employee.Roles;

                    // Cập nhật ảnh nếu có
                    if (uploadPhoto != null)
                    {
                        string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/Employee");
                        string fileName = Path.GetFileName(uploadPhoto.FileName);
                        string filePath = Path.Combine(uploadsFolder, fileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await uploadPhoto.CopyToAsync(fileStream);
                        }
                        existingEmployee.Photo = fileName;
                    }

                    // Giữ nguyên mật khẩu nếu không nhập mật khẩu mới (giả sử bạn có trường Password trong form)
                    if (!string.IsNullOrEmpty(employee.Password))
                    {
                        existingEmployee.Password = employee.Password;
                    }

                    _context.Employees.Update(existingEmployee);
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
