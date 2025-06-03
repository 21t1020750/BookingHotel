using Microsoft.AspNetCore.Mvc;
using BookingHotel.Areas.Admin.Data;
using BookingHotel.Areas.Admin.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingHotel.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TagsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TagsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/Tags
        public async Task<IActionResult> Index()
        {
            var tags = await _context.Tags.ToListAsync();
            return View(tags);
        }

        // GET: /Admin/Tags/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Admin/Tags/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TagName")] Tag tag)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tag);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tag đã được thêm thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(tag);
        }

        // GET: /Admin/Tags/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag == null)
            {
                return NotFound();
            }
            return View(tag);
        }

        // POST: /Admin/Tags/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TagID,TagName")] Tag tag)
        {
            if (id != tag.TagID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tag);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Tag đã được cập nhật thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TagExists(tag.TagID))
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
            return View(tag);
        }

        // GET: /Admin/Tags/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag == null)
            {
                return NotFound();
            }
            return View(tag);
        }

        // POST: /Admin/Tags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag != null)
            {
                _context.Tags.Remove(tag);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tag đã được xóa thành công!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool TagExists(int id)
        {
            return _context.Tags.Any(e => e.TagID == id);
        }
    }
}