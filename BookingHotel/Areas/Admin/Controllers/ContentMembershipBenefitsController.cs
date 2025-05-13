using BookingHotel.Areas.Admin.Data;
using BookingHotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingHotel.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ContentMembershipBenefitsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ContentMembershipBenefitsController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: /Admin/ContentMembershipBenefits
        public async Task<IActionResult> Index()
        {
            var benefits = await _db.Content_MembershipBenefits.ToListAsync();
            return View(benefits);
        }

        // GET: /Admin/ContentMembershipBenefits/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Admin/ContentMembershipBenefits/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Content_MembershipBenefit model) // Đổi từ 'benefit' thành 'model'
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _db.Content_MembershipBenefits.Add(model);
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi lưu dữ liệu: " + ex.Message);
                }
            }
            return View(model);
        }

        // GET: /Admin/ContentMembershipBenefits/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var benefit = await _db.Content_MembershipBenefits.FindAsync(id);
            if (benefit == null)
            {
                return NotFound();
            }
            return View(benefit);
        }

        // POST: /Admin/ContentMembershipBenefits/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Content_MembershipBenefit model) // Đổi từ 'benefit' thành 'model'
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(model);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BenefitExists(model.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi lưu dữ liệu: " + ex.Message);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: /Admin/ContentMembershipBenefits/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var benefit = await _db.Content_MembershipBenefits
                .FirstOrDefaultAsync(m => m.Id == id);
            if (benefit == null)
            {
                return NotFound();
            }

            return View(benefit);
        }

        // POST: /Admin/ContentMembershipBenefits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var benefit = await _db.Content_MembershipBenefits.FindAsync(id);
            if (benefit != null)
            {
                _db.Content_MembershipBenefits.Remove(benefit);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool BenefitExists(int id)
        {
            return _db.Content_MembershipBenefits.Any(e => e.Id == id);
        }
    }
}