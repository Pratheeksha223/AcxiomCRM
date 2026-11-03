
using AcxiomCRM.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers
{
    public class ReportsController : Controller
    {
        private readonly AppDbContext _context;

        public ReportsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalCustomers =
                await _context.Customers.CountAsync();

            ViewBag.TotalLeads =
                await _context.Leads.CountAsync();

            ViewBag.TotalOpportunities =
                await _context.Opportunities.CountAsync();

            ViewBag.TotalFollowUps =
                await _context.FollowUps.CountAsync();

            ViewBag.TotalDealValue =
                await _context.Opportunities
                    .SumAsync(o => (decimal?)o.DealValue) ?? 0m;

            ViewBag.GeneratedOn = DateTime.Now;

            return View();
        }
    }
}
