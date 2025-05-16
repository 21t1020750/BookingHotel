using Microsoft.AspNetCore.Mvc;

namespace BookingHotel.Controllers
{
    public class LocationController : Controller
    {
        public IActionResult Details(int id)
        {
            // Giả lập dữ liệu địa điểm (thay bằng logic lấy dữ liệu từ database)
            var location = new LocationViewModel
            {
                Id = id,
                Title = id switch
                {
                    1 => "Hồ Bơi Sang Trọng",
                    2 => "Nhà Hàng 5 Sao",
                    3 => "Phòng Gym Hiện Đại",
                    4 => "Spa Thư Giãn",
                    _ => "Địa Điểm Không Xác Định"
                },
                Description = "Mô tả chi tiết về địa điểm này: Đây là nơi lý tưởng để thư giãn và tận hưởng kỳ nghỉ của bạn.",
                ImageUrl = id switch
                {
                    1 => "https://images.unsplash.com/photo-1600585154340-be6161a56a0c",
                    2 => "https://images.unsplash.com/photo-1542314831-068cd1dbfeeb",
                    3 => "https://images.unsplash.com/photo-1593297237733-184d3ff53d80",
                    4 => "https://du-lich.chudu24.com/f/m/2304/24/khach-san-hoang-hon-tim-boutique-phu-quoc-2.jpg",
                    _ => "https://images.unsplash.com/photo-1600585154340-be6161a56a0c"
                }
            };

            return View(location);
        }
    }

    // Model tạm thời cho địa điểm
    public class LocationViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
    }
}