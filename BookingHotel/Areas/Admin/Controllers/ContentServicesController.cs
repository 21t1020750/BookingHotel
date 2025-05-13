using BookingHotel.Areas.Admin.Data;
using BookingHotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingHotel.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ContentServicesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ContentServicesController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: /Admin/ContentServices
        public async Task<IActionResult> Index()
        {
            var services = await _db.Content_Services.ToListAsync();
            return View(services);
        }

        // GET: /Admin/ContentServices/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Admin/ContentServices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Content_Service service)
        {
            if (ModelState.IsValid)
            {
                _db.Content_Services.Add(service);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(service);
        }

        // GET: /Admin/ContentServices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _db.Content_Services.FindAsync(id);
            if (service == null)
            {
                return NotFound();
            }
            return View(service);
        }

        // POST: /Admin/ContentServices/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Content_Service service)
        {
            if (id != service.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(service);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceExists(service.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(service);
        }

        // GET: /Admin/ContentServices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _db.Content_Services
                .FirstOrDefaultAsync(m => m.Id == id);
            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        // POST: /Admin/ContentServices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var service = await _db.Content_Services.FindAsync(id);
            if (service != null)
            {
                _db.Content_Services.Remove(service);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ServiceExists(int id)
        {
            return _db.Content_Services.Any(e => e.Id == id);
        }
    }
}