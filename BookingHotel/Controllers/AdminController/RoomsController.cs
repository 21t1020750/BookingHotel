using BookingHotel.Data;
using BookingHotel.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BookingHotel.Controllers.AdminController
{
    public class RoomsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const string DefaultPhoto = "nophoto.png";

        public RoomsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var rooms = _context.Rooms
                .Include(r => r.RoomType)
                .Include(r => r.RoomStatus)
                .ToList();
            return View(rooms);
        }

        public IActionResult Create()
        {
            ViewBag.RoomTypes = new SelectList(_context.RoomTypes, "RoomTypeID", "TypeName");
            return View("Edit", new Room { Photo = DefaultPhoto });
        }

        public IActionResult Edit(int? id)
        {
            Room room = id == null || id == 0
                ? new Room { RoomID = 0, Photo = DefaultPhoto }
                : _context.Rooms.Include(r => r.RoomType)
                                .Include(r => r.RoomStatus)
                                .FirstOrDefault(r => r.RoomID == id);

            if (room == null)
                return NotFound();

            ViewBag.Title = id == 0 ? "Thêm phòng mới" : "Chỉnh sửa phòng";
            if (string.IsNullOrEmpty(room.Photo))
            {
                room.Photo = DefaultPhoto;
            }
            ViewBag.RoomTypes = new SelectList(_context.RoomTypes, "RoomTypeID", "TypeName", room.RoomTypeID);
            return View(room);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Room room, IFormFile uploadPhoto)
        {
            // Validate if the room type is selected
            if (room.RoomTypeID == 0)
            {
                ModelState.AddModelError("RoomTypeID", "Vui lòng chọn loại phòng.");
            }

            // Validate if the room status is selected
            if (room.StatusID == 0)
            {
                ModelState.AddModelError("StatusID", "Vui lòng chọn trạng thái phòng.");
            }

            // Validate if the room price is set (greater than 0)
            if (room.Price <= 0)
            {
                ModelState.AddModelError("Price", "Giá phòng phải lớn hơn 0.");
            }

            // Check for duplicate room name if it's a new room
            if (_context.Rooms.Any(r => r.RoomNumber == room.RoomNumber && r.RoomID != room.RoomID))
            {
                ModelState.AddModelError("RoomNumber", "Tên phòng này đã tồn tại.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/room");

                    // If the user uploads a new image
                    if (uploadPhoto != null && uploadPhoto.Length > 0)
                    {
                        string filePath = Path.Combine(uploadsFolder, uploadPhoto.FileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await uploadPhoto.CopyToAsync(fileStream);
                        }

                        room.Photo = uploadPhoto.FileName;
                    }
                    else
                    {
                        if (room.RoomID == 0)
                        {
                            room.Photo = "nophoto.png";
                        }
                        else
                        {
                            var existingRoom = _context.Rooms.AsNoTracking().FirstOrDefault(r => r.RoomID == room.RoomID);
                            room.Photo = existingRoom?.Photo ?? "nophoto.png";
                        }
                    }

                    // Add or update room
                    if (room.RoomID == 0)
                    {
                        _context.Rooms.Add(room);
                    }
                    else
                    {
                        _context.Rooms.Update(room);
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that occur during the save
                    ModelState.AddModelError("", "Đã có lỗi xảy ra: " + ex.Message);
                }
            }

            ViewBag.RoomTypes = new SelectList(_context.RoomTypes, "RoomTypeID", "TypeName", room.RoomTypeID);
            return View(room);
        }





        public IActionResult Delete(int id)
        {
            var room = _context.Rooms.Find(id);
            if (room != null)
            {
                if (!string.IsNullOrEmpty(room.Photo) && room.Photo != DefaultPhoto)
                {
                    string filePath = Path.Combine("wwwroot", room.Photo.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                        System.IO.File.Delete(filePath);
                }

                _context.Rooms.Remove(room);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}