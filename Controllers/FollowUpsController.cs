
using AcxiomCRM.Data;
using AcxiomCRM.Models;
using AcxiomCRM.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers
{
    public class FollowUpsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IAuditLogService _auditLogService;

        public FollowUpsController(
            AppDbContext context,
            IAuditLogService auditLogService)
        {
            _context = context;
            _auditLogService = auditLogService;
        }

        // Display all follow-ups and search
        public async Task<IActionResult> Index(string searchString)
        {
            var followUps = _context.FollowUps.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                followUps = followUps.Where(f =>
                    f.CustomerName.Contains(searchString) ||
                    f.Subject.Contains(searchString) ||
                    f.Status.Contains(searchString));
            }

            return View(await followUps
                .OrderByDescending(f => f.FollowUpDate)
                .ToListAsync());
        }

        // Create GET
        public IActionResult Create()
        {
            return View();
        }

        // Create POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FollowUp followUp)
        {
            if (ModelState.IsValid)
            {
                followUp.CreatedDate = DateTime.UtcNow;

                _context.FollowUps.Add(followUp);
                await _context.SaveChangesAsync();

                await _auditLogService.LogAsync(
                    "Created",
                    "Follow-up",
                    $"Follow-up '{followUp.Subject}' was created for customer '{followUp.CustomerName}'."
                );

                TempData["Success"] = "Follow-up added successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(followUp);
        }

        // Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var followUp = await _context.FollowUps
                .FirstOrDefaultAsync(f => f.Id == id);

            if (followUp == null) return NotFound();

            return View(followUp);
        }

        // Edit GET
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var followUp = await _context.FollowUps.FindAsync(id);

            if (followUp == null) return NotFound();

            return View(followUp);
        }

        // Edit POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FollowUp followUp)
        {
            if (id != followUp.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(followUp);
                    await _context.SaveChangesAsync();

                    await _auditLogService.LogAsync(
                        "Updated",
                        "Follow-up",
                        $"Follow-up '{followUp.Subject}' for customer '{followUp.CustomerName}' was updated."
                    );

                    TempData["Success"] = "Follow-up updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.FollowUps.AnyAsync(f => f.Id == id))
                        return NotFound();

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(followUp);
        }

        // Delete GET
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var followUp = await _context.FollowUps
                .FirstOrDefaultAsync(f => f.Id == id);

            if (followUp == null) return NotFound();

            return View(followUp);
        }

        // Delete POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var followUp = await _context.FollowUps.FindAsync(id);

            if (followUp != null)
            {
                var subject = followUp.Subject;
                var customerName = followUp.CustomerName;

                _context.FollowUps.Remove(followUp);
                await _context.SaveChangesAsync();

                await _auditLogService.LogAsync(
                    "Deleted",
                    "Follow-up",
                    $"Follow-up '{subject}' for customer '{customerName}' was deleted."
                );
            }

            TempData["Success"] = "Follow-up deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
