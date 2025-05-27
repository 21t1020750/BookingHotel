using BookingHotel.Areas.Admin.Data;
using BookingHotel.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BookingHotel.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, employee")]
    public class RoomsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private const string DefaultPhoto = "nophoto.png";

        public RoomsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var rooms = _context.Rooms
                .Include(r => r.RoomType)
                .Include(r => r.RoomStatus)
                .Include(r => r.RoomImages)
                .ToList();
            return View(rooms);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung phòng";
            ViewBag.RoomTypes = new SelectList(_context.RoomTypes, "RoomTypeID", "TypeName");
            return View("Edit", new Room { Photo = DefaultPhoto });
        }

        public IActionResult Edit(int? id)
        {
            Room room = id == null || id == 0
                ? new Room { RoomID = 0, Photo = DefaultPhoto, RoomImages = new List<RoomImage>() }
                : _context.Rooms.Include(r => r.RoomType)
                                .Include(r => r.RoomStatus)
                                .Include(r => r.RoomImages)
                                .FirstOrDefault(r => r.RoomID == id);

            if (room == null)
                return NotFound();

            ViewBag.Title = "Chỉnh sửa phòng";

            ViewBag.RoomTypes = new SelectList(_context.RoomTypes, "RoomTypeID", "TypeName", room.RoomTypeID);
            return View(room);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Room room, List<RoomImage> UploadedImages, List<IFormFile> NewImages, IFormFile uploadPhoto, List<string> RoomImages)
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

                    // Cập nhật hoặc thêm room
                    if (room.RoomID == 0)
                        _context.Rooms.Add(room);
                    else
                        _context.Rooms.Update(room);

                    await _context.SaveChangesAsync();


                    // Xử lý ảnh
                    if (UploadedImages != null)
                    {
                        foreach (var img in UploadedImages)
                        {
                            var existing = await _context.RoomImages
                                .FirstOrDefaultAsync(r => r.ImagePath == img.ImagePath && r.RoomID == room.RoomID);

                            if (existing != null)
                            {
                                existing.DisplayOrder = img.DisplayOrder;
                                _context.RoomImages.Update(existing);
                            }
                        }
                        await _context.SaveChangesAsync();
                    }

                    // Thêm ảnh mới
                    if (NewImages != null && NewImages.Count > 0)
                    {
                        foreach (var file in NewImages)
                        {
                            if (file.Length > 0)
                            {
                                string folderPath = Path.Combine("wwwroot", "images", "room", "ExtraImages");
                                if (!Directory.Exists(folderPath))
                                    Directory.CreateDirectory(folderPath);

                                string fileName = Path.GetFileName(file.FileName);
                                string filePath = Path.Combine(folderPath, fileName);

                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await file.CopyToAsync(stream);
                                }

                                RoomImage newImage = new RoomImage
                                {
                                    RoomID = room.RoomID,
                                    ImagePath = Path.Combine("images", "room", "ExtraImages", fileName).Replace("\\", "/"),
                                    DisplayOrder = 0
                                };

                                _context.RoomImages.Add(newImage);
                            }
                        }

                        await _context.SaveChangesAsync();
                    }

                    if (RoomImages != null && RoomImages.Any())
                    {
                        foreach (var imagePath in RoomImages)
                        {
                            var roomImage = new RoomImage
                            {
                                RoomID = room.RoomID,
                                ImagePath = imagePath
                            };
                            _context.RoomImages.Add(roomImage);
                        }
                        await _context.SaveChangesAsync();
                    }

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi lưu: " + ex.Message);
                }
            }

            ViewBag.RoomTypes = new SelectList(_context.RoomTypes, "RoomTypeID", "TypeName", room.RoomTypeID);
            return View(room);
        }


        [HttpPost]
        public async Task<IActionResult> UploadTempImages(List<IFormFile> files)
        {
            var uploadResults = new List<string>();

            if (files != null && files.Any())
            {
                var tempFolder = Path.Combine("wwwroot", "images", "room");
                Directory.CreateDirectory(tempFolder);

                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName); // Giữ tên gốc
                        var path = Path.Combine(tempFolder, fileName);

                        using (var stream = new FileStream(path, FileMode.Create)) // Ghi đè nếu đã tồn tại
                        {
                            await file.CopyToAsync(stream);
                        }

                        // Trả về đường dẫn tương đối
                        uploadResults.Add($"images/room/{fileName}");
                    }
                }
            }

            return Json(uploadResults);
        }


        [HttpPost]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var image = await _context.RoomImages.FindAsync(id);
            if (image != null)
            {
                // Xóa file vật lý
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, image.ImagePath.TrimStart('~', '/').Replace("/", "\\"));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                // Xóa DB
                _context.RoomImages.Remove(image);
                await _context.SaveChangesAsync();
            }

            // Redirect về Edit room
            return RedirectToAction("Edit", new { id = image.RoomID });
        }

        public IActionResult Delete(int id)
        {
            var room = _context.Rooms.Find(id);
            if (room != null)
            {
                _context.Rooms.Remove(room);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IActionResult Detail(int id)
        {
            var room = _context.Rooms.Find(id);
            if (room == null)
                return NotFound();

            // Lấy tất cả BookingDetails có RoomID = id
            var bookingDetails = _context.BookingDetails.Where(bd => bd.RoomID == id);

            // Lấy tất cả Reviews liên quan tới các BookingID của bookingDetails
            var ratings = _context.Reviews
                            .Where(rv => bookingDetails.Select(bd => bd.BookingID).Contains(rv.BookingID))
                            .Select(rv => rv.Rating);

            double avgRating = 0;
            if (ratings.Any())
            {
                avgRating = ratings.Average();
            }

            ViewBag.AverageRating = Math.Round(avgRating, 2);

            return View(room);
        }
    }
}