
using AcxiomCRM.Data;
using AcxiomCRM.Models;
using AcxiomCRM.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers
{
    public class OpportunitiesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IAuditLogService _auditLogService;

        public OpportunitiesController(
            AppDbContext context,
            IAuditLogService auditLogService)
        {
            _context = context;
            _auditLogService = auditLogService;
        }

        // GET: Opportunities
        public async Task<IActionResult> Index(string searchString)
        {
            var opportunities = _context.Opportunities.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                opportunities = opportunities.Where(o =>
                    o.OpportunityName.Contains(searchString) ||
                    o.CustomerName.Contains(searchString) ||
                    o.CompanyName.Contains(searchString));
            }

            return View(await opportunities
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync());
        }

        // GET: Opportunities/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Opportunities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Opportunity opportunity)
        {
            if (ModelState.IsValid)
            {
                opportunity.CreatedDate = DateTime.UtcNow;

                _context.Opportunities.Add(opportunity);
                await _context.SaveChangesAsync();

                await _auditLogService.LogAsync(
                    "Created",
                    "Opportunity",
                    $"Opportunity '{opportunity.OpportunityName}' was created. Customer: {opportunity.CustomerName}, Deal Value: {opportunity.DealValue}"
                );

                TempData["Success"] = "Opportunity added successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(opportunity);
        }

        // GET: Opportunities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var opportunity = await _context.Opportunities
                .FirstOrDefaultAsync(o => o.Id == id);

            if (opportunity == null)
                return NotFound();

            return View(opportunity);
        }

        // GET: Opportunities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var opportunity = await _context.Opportunities.FindAsync(id);

            if (opportunity == null)
                return NotFound();

            return View(opportunity);
        }

        // POST: Opportunities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id, Opportunity opportunity)
        {
            if (id != opportunity.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.Opportunities.FindAsync(id);

                    if (existing == null)
                        return NotFound();

                    existing.OpportunityName = opportunity.OpportunityName;
                    existing.CustomerName = opportunity.CustomerName;
                    existing.CompanyName = opportunity.CompanyName;
                    existing.DealValue = opportunity.DealValue;
                    existing.Stage = opportunity.Stage;
                    existing.Status = opportunity.Status;
                    existing.ExpectedCloseDate = opportunity.ExpectedCloseDate;
                    existing.Description = opportunity.Description;

                    await _context.SaveChangesAsync();

                    await _auditLogService.LogAsync(
                        "Updated",
                        "Opportunity",
                        $"Opportunity '{existing.OpportunityName}' was updated. Customer: {existing.CustomerName}, Deal Value: {existing.DealValue}"
                    );

                    TempData["Success"] = "Opportunity updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.Opportunities.AnyAsync(e => e.Id == id))
                        return NotFound();

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(opportunity);
        }

        // GET: Opportunities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var opportunity = await _context.Opportunities
                .FirstOrDefaultAsync(o => o.Id == id);

            if (opportunity == null)
                return NotFound();

            return View(opportunity);
        }

        // POST: Opportunities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var opportunity = await _context.Opportunities.FindAsync(id);

            if (opportunity != null)
            {
                var opportunityName = opportunity.OpportunityName;
                var customerName = opportunity.CustomerName;

                _context.Opportunities.Remove(opportunity);
                await _context.SaveChangesAsync();

                await _auditLogService.LogAsync(
                    "Deleted",
                    "Opportunity",
                    $"Opportunity '{opportunityName}' belonging to customer '{customerName}' was deleted."
                );
            }

            TempData["Success"] = "Opportunity deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
