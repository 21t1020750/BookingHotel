using Microsoft.AspNetCore.Mvc;
using BookingHotel.Areas.Admin.Data;
using BookingHotel.Areas.Admin.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingHotel.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DiningDishesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DiningDishesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/DiningDishes
        public async Task<IActionResult> Index()
        {
            var dishes = await _context.Dishes.ToListAsync();
            return View(dishes);
        }

        // GET: /Admin/DiningDishes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Admin/DiningDishes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Label,ImageURL")] Dish dish)
        {
            if (ModelState.IsValid)
            {
                dish.CreatedAt = DateTime.Now;
                _context.Add(dish);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Món ăn đã được thêm thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(dish);
        }

        // GET: /Admin/DiningDishes/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var dish = await _context.Dishes.FindAsync(id);
            if (dish == null)
            {
                return NotFound();
            }
            return View(dish);
        }

        // POST: /Admin/DiningDishes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DishID,Name,Description,Label,ImageURL,CreatedAt")] Dish dish)
        {
            if (id != dish.DishID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dish);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Món ăn đã được cập nhật thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DishExists(dish.DishID))
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
            return View(dish);
        }

        // GET: /Admin/DiningDishes/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var dish = await _context.Dishes.FindAsync(id);
            if (dish == null)
            {
                return NotFound();
            }
            return View(dish);
        }

        // POST: /Admin/DiningDishes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dish = await _context.Dishes.FindAsync(id);
            if (dish != null)
            {
                _context.Dishes.Remove(dish);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Món ăn đã được xóa thành công!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool DishExists(int id)
        {
            return _context.Dishes.Any(e => e.DishID == id);
        }
    }
}