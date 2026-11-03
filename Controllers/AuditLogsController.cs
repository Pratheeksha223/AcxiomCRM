
using AcxiomCRM.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers
{
    public class AuditLogsController : Controller
    {
        private readonly AppDbContext _context;

        public AuditLogsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string search)
        {
            var logs = _context.AuditLogs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                logs = logs.Where(log =>
                    log.UserName.Contains(search) ||
                    log.Action.Contains(search) ||
                    log.EntityName.Contains(search) ||
                    log.Details.Contains(search));
            }

            ViewBag.Search = search;

            var result = await logs
                .OrderByDescending(log => log.Timestamp)
                .Take(200)
                .ToListAsync();

            return View(result);
        }
    }
}
