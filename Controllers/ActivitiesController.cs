
using AcxiomCRM.Data;
using AcxiomCRM.Models;
using AcxiomCRM.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers
{
    public class ActivitiesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IAuditLogService _auditLogService;

        public ActivitiesController(
            AppDbContext context,
            IAuditLogService auditLogService)
        {
            _context = context;
            _auditLogService = auditLogService;
        }

        // Display all activities
        public async Task<IActionResult> Index()
        {
            var activities = await _context.Activities
                .OrderByDescending(a => a.ActivityDate)
                .ToListAsync();

            return View(activities);
        }

        // Create GET
        public IActionResult Create()
        {
            return View();
        }

        // Create POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Activity activity)
        {
            if (ModelState.IsValid)
            {
                _context.Activities.Add(activity);
                await _context.SaveChangesAsync();

                await _auditLogService.LogAsync(
                    "Created",
                    "Activity",
                    $"Activity '{activity.ActivityName}' was created for customer '{activity.CustomerName}'."
                );

                TempData["Success"] = "Activity added successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(activity);
        }

        // Details GET
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var activity = await _context.Activities
                .FirstOrDefaultAsync(a => a.Id == id);

            if (activity == null)
                return NotFound();

            return View(activity);
        }

        // Edit GET
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var activity = await _context.Activities.FindAsync(id);

            if (activity == null)
                return NotFound();

            return View(activity);
        }

        // Edit POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Activity activity)
        {
            if (id != activity.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(activity);

            var existing = await _context.Activities.FindAsync(id);

            if (existing == null)
                return NotFound();

            existing.ActivityName = activity.ActivityName;
            existing.CustomerName = activity.CustomerName;
            existing.Type = activity.Type;
            existing.ActivityDate = activity.ActivityDate;
            existing.Status = activity.Status;
            existing.Description = activity.Description;

            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "Updated",
                "Activity",
                $"Activity '{existing.ActivityName}' for customer '{existing.CustomerName}' was updated."
            );

            TempData["Success"] = "Activity updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // Delete GET
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var activity = await _context.Activities
                .FirstOrDefaultAsync(a => a.Id == id);

            if (activity == null)
                return NotFound();

            return View(activity);
        }

        // Delete POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var activity = await _context.Activities.FindAsync(id);

            if (activity == null)
                return NotFound();

            var activityName = activity.ActivityName;
            var customerName = activity.CustomerName;

            _context.Activities.Remove(activity);
            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "Deleted",
                "Activity",
                $"Activity '{activityName}' for customer '{customerName}' was deleted."
            );

            TempData["Success"] = "Activity deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
