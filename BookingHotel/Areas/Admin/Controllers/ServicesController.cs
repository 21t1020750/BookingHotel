using BookingHotel.Areas.Admin.Data;
using BookingHotel.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingHotel.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ServicesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServicesController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var service = _context.Services.ToList();
            return View(service);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung Dịch Vụ";
            return View("Edit", new Service());  // Truyền đối tượng mới cho view Edit
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = _context.Services.Find(id);
            if (service == null)
            {
                return NotFound();
            }

            ViewBag.Title = "Chỉnh sửa phòng";
            return View("Edit", service);  // Truyền đối tượng tìm được cho view Edit
        }

        // POST: Create or Edit
        [HttpPost]
        public IActionResult Edit(int? id, Service service)
        {
            if (id != null && id != service.ServiceID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (id == null) // Nếu không có id, tức là tạo mới
                {
                    _context.Add(service);
                }
                else // Nếu có id, tức là chỉnh sửa
                {
                    _context.Update(service);
                }

                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View("Edit", service); // Trả về lại view với dữ liệu hiện tại nếu có lỗi
        }

        // Xóa phòng
        public IActionResult Delete(int id)
        {
            var service = _context.Services.Find(id);
            if (service != null)
            {
                _context.Services.Remove(service);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}

