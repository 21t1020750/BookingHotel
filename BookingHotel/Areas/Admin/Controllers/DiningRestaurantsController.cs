using Microsoft.AspNetCore.Mvc;
using BookingHotel.Areas.Admin.Data;
using BookingHotel.Areas.Admin.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingHotel.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DiningRestaurantsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DiningRestaurantsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/DiningRestaurants
        public async Task<IActionResult> Index()
        {
            var restaurants = await _context.Restaurants
                .Select(r => new RestaurantViewModel
                {
                    Restaurant = r,
                    Tags = _context.Restaurant_Tags
                        .Where(rt => rt.RestaurantID == r.RestaurantID)
                        .Join(_context.Tags,
                              rt => rt.TagID,
                              t => t.TagID,
                              (rt, t) => t.TagName)
                        .ToList()
                })
                .ToListAsync();
            return View(restaurants);
        }

        // GET: /Admin/DiningRestaurants/Create
        public IActionResult Create()
        {
            ViewBag.Tags = _context.Tags.ToList();
            return View();
        }

        // POST: /Admin/DiningRestaurants/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,OperatingHours,ImageURL")] Restaurant restaurant, int[] selectedTagIds)
        {
            if (ModelState.IsValid)
            {
                restaurant.CreatedAt = DateTime.Now;
                _context.Add(restaurant);
                await _context.SaveChangesAsync();

                // Add selected tags to RestaurantTag
                if (selectedTagIds != null)
                {
                    foreach (var tagId in selectedTagIds)
                    {
                        _context.Restaurant_Tags.Add(new RestaurantTag
                        {
                            RestaurantID = restaurant.RestaurantID,
                            TagID = tagId
                        });
                    }
                    await _context.SaveChangesAsync();
                }

                TempData["SuccessMessage"] = "Nhà hàng đã được thêm thành công!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Tags = _context.Tags.ToList(); // Repopulate tags if validation fails
            return View(restaurant);
        }
        // GET: /Admin/DiningRestaurants/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var restaurant = await _context.Restaurants.FindAsync(id);
            if (restaurant == null)
            {
                return NotFound();
            }
            ViewBag.Tags = _context.Tags.ToList(); // Pass list of tags
            var selectedTags = await _context.Restaurant_Tags
                .Where(rt => rt.RestaurantID == id)
                .Select(rt => rt.TagID)
                .ToListAsync();
            ViewBag.SelectedTagIds = selectedTags; // Pass selected tags
            return View(restaurant);
        }

        // POST: /Admin/DiningRestaurants/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RestaurantID,Name,Description,OperatingHours,ImageURL,CreatedAt")] Restaurant restaurant, int[] selectedTagIds)
        {
            if (id != restaurant.RestaurantID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(restaurant);
                    // Update RestaurantTag
                    var existingTags = await _context.Restaurant_Tags
                        .Where(rt => rt.RestaurantID == restaurant.RestaurantID)
                        .ToListAsync();
                    _context.Restaurant_Tags.RemoveRange(existingTags); // Remove old tags

                    if (selectedTagIds != null)
                    {
                        foreach (var tagId in selectedTagIds)
                        {
                            _context.Restaurant_Tags.Add(new RestaurantTag
                            {
                                RestaurantID = restaurant.RestaurantID,
                                TagID = tagId
                            });
                        }
                    }
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Nhà hàng đã được cập nhật thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RestaurantExists(restaurant.RestaurantID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Tags = _context.Tags.ToList(); // Repopulate tags if validation fails
            ViewBag.SelectedTagIds = selectedTagIds ?? new int[] { }; // Repopulate selected tags
            return View(restaurant);
        }

        // GET: /Admin/DiningRestaurants/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var restaurant = await _context.Restaurants.FindAsync(id);
            if (restaurant == null)
            {
                return NotFound();
            }
            return View(restaurant);
        }

        // POST: /Admin/DiningRestaurants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var restaurant = await _context.Restaurants.FindAsync(id);
            if (restaurant != null)
            {
                _context.Restaurants.Remove(restaurant);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Nhà hàng đã được xóa thành công!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool RestaurantExists(int id)
        {
            return _context.Restaurants.Any(e => e.RestaurantID == id);
        }
    }
}