
using AcxiomCRM.Data;
using AcxiomCRM.Models;
using AcxiomCRM.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers
{
    public class CustomerController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IAuditLogService _auditLogService;

        public CustomerController(
            AppDbContext context,
            IAuditLogService auditLogService)
        {
            _context = context;
            _auditLogService = auditLogService;
        }

        // Display all customers and search
        public async Task<IActionResult> Index(string search)
        {
            var customers = _context.Customers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                customers = customers.Where(c =>
                    c.CustomerName.Contains(search) ||
                    c.Email.Contains(search) ||
                    c.CompanyName.Contains(search));
            }

            ViewBag.Search = search;

            return View(await customers
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync());
        }

        // View customer details
        public async Task<IActionResult> Details(int id)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
                return NotFound();

            return View(customer);
        }

        // Display create form
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Save new customer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer customer)
        {
            if (!ModelState.IsValid)
                return View(customer);

            try
            {
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                await _auditLogService.LogAsync(
                    "Created",
                    "Customer",
                    $"Customer '{customer.CustomerName}' was created. Email: {customer.Email}"
                );

                TempData["Success"] = "Customer added successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("",
                    "Email or phone number already exists.");
                return View(customer);
            }
        }

        // Display edit form
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
                return NotFound();

            return View(customer);
        }

        // Update customer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Customer customer)
        {
            if (id != customer.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(customer);

            try
            {
                _context.Update(customer);
                await _context.SaveChangesAsync();

                await _auditLogService.LogAsync(
                    "Updated",
                    "Customer",
                    $"Customer '{customer.CustomerName}' was updated. Email: {customer.Email}"
                );

                TempData["Success"] = "Customer updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("",
                    "Email or phone number already exists.");
                return View(customer);
            }
        }

        // Delete customer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
                return NotFound();

            var customerName = customer.CustomerName;
            var customerEmail = customer.Email;

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "Deleted",
                "Customer",
                $"Customer '{customerName}' was deleted. Email: {customerEmail}"
            );

            TempData["Success"] = "Customer deleted successfully!";

            return RedirectToAction(nameof(Index));
        }
    }
}
