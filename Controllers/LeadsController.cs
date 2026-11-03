
using AcxiomCRM.Data;
using AcxiomCRM.Models;
using AcxiomCRM.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers
{
    public class LeadsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IAuditLogService _auditLogService;

        public LeadsController(
            AppDbContext context,
            IAuditLogService auditLogService)
        {
            _context = context;
            _auditLogService = auditLogService;
        }

        // Display and search leads
        public async Task<IActionResult> Index(string searchString)
        {
            var leads = _context.Leads.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                leads = leads.Where(l =>
                    l.LeadName.Contains(searchString) ||
                    l.Email.Contains(searchString) ||
                    l.CompanyName.Contains(searchString));
            }

            ViewBag.SearchString = searchString;

            return View(await leads
                .OrderByDescending(l => l.CreatedDate)
                .ToListAsync());
        }

        // View lead details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var lead = await _context.Leads
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lead == null) return NotFound();

            return View(lead);
        }

        // Add lead
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Lead lead)
        {
            if (ModelState.IsValid)
            {
                lead.CreatedDate = DateTime.UtcNow;

                _context.Leads.Add(lead);
                await _context.SaveChangesAsync();

                await _auditLogService.LogAsync(
                    "Created",
                    "Lead",
                    $"Lead '{lead.LeadName}' was created. Email: {lead.Email}"
                );

                TempData["Success"] = "Lead added successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(lead);
        }

        // Edit lead
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var lead = await _context.Leads.FindAsync(id);

            if (lead == null) return NotFound();

            return View(lead);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Lead lead)
        {
            if (id != lead.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lead);
                    await _context.SaveChangesAsync();

                    await _auditLogService.LogAsync(
                        "Updated",
                        "Lead",
                        $"Lead '{lead.LeadName}' was updated. Email: {lead.Email}"
                    );

                    TempData["Success"] = "Lead updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.Leads.AnyAsync(
                        l => l.Id == lead.Id))
                        return NotFound();

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(lead);
        }

        // Delete lead
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var lead = await _context.Leads
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lead == null) return NotFound();

            return View(lead);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lead = await _context.Leads.FindAsync(id);

            if (lead != null)
            {
                var leadName = lead.LeadName;
                var leadEmail = lead.Email;

                _context.Leads.Remove(lead);
                await _context.SaveChangesAsync();

                await _auditLogService.LogAsync(
                    "Deleted",
                    "Lead",
                    $"Lead '{leadName}' was deleted. Email: {leadEmail}"
                );
            }

            TempData["Success"] = "Lead deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
