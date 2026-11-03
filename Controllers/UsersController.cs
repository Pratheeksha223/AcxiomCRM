
using AcxiomCRM.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAuditLogService _auditLogService;

        public UsersController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IAuditLogService auditLogService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _auditLogService = auditLogService;
        }

        // Display users
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users
                .OrderBy(u => u.UserName)
                .ToListAsync();

            var userDetails = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                userDetails.Add(new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email ?? "",
                    UserName = user.UserName ?? "",
                    Roles = string.Join(", ", roles)
                });
            }

            return View(userDetails);
        }

        // Create GET
        public IActionResult Create()
        {
            return View();
        }

        // Create POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            string email,
            string password,
            string confirmPassword,
            string role)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Email and password are required.");
                return View();
            }

            if (password != confirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match.");
                return View();
            }

            if (role != "Admin" && role != "Employee")
            {
                ModelState.AddModelError("", "Invalid role selected.");
                return View();
            }

            if (!await _roleManager.RoleExistsAsync(role))
            {
                ModelState.AddModelError("", "Selected role does not exist.");
                return View();
            }

            var existingUser = await _userManager.FindByEmailAsync(email);

            if (existingUser != null)
            {
                ModelState.AddModelError("", "Email already exists.");
                return View();
            }

            var user = new IdentityUser
            {
                UserName = email,
                Email = email
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(user, role);

                if (!roleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(user);

                    foreach (var error in roleResult.Errors)
                        ModelState.AddModelError("", error.Description);

                    return View();
                }

                await _auditLogService.LogAsync(
                    "Created",
                    "User",
                    $"User '{email}' was created with role '{role}'."
                );

                TempData["Success"] = "User created successfully!";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View();
        }

        // Edit GET
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            return View(new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email ?? "",
                Role = roles.FirstOrDefault() ?? "Employee"
            });
        }

        // Edit POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            string id, string email, string role)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(email) ||
                !new System.ComponentModel.DataAnnotations.EmailAddressAttribute()
                    .IsValid(email))
            {
                ModelState.AddModelError("", "Enter a valid email address.");
                return View(new EditUserViewModel
                {
                    Id = id,
                    Email = email,
                    Role = role
                });
            }

            if (role != "Admin" && role != "Employee")
            {
                ModelState.AddModelError("", "Invalid role selected.");
                return View(new EditUserViewModel
                {
                    Id = id,
                    Email = email,
                    Role = role
                });
            }

            if (!await _roleManager.RoleExistsAsync(role))
            {
                ModelState.AddModelError("", "Selected role does not exist.");
                return View(new EditUserViewModel
                {
                    Id = id,
                    Email = email,
                    Role = role
                });
            }

            var duplicate = await _userManager.FindByEmailAsync(email);

            if (duplicate != null && duplicate.Id != id)
            {
                ModelState.AddModelError("", "Email already exists.");
                return View(new EditUserViewModel
                {
                    Id = id,
                    Email = email,
                    Role = role
                });
            }

            var oldEmail = user.Email ?? "";
            var oldRoles = await _userManager.GetRolesAsync(user);
            var oldRoleText = string.Join(", ", oldRoles);

            user.Email = email;
            user.UserName = email;

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                    ModelState.AddModelError("", error.Description);

                return View(new EditUserViewModel
                {
                    Id = id,
                    Email = email,
                    Role = role
                });
            }

            if (oldRoles.Any())
                await _userManager.RemoveFromRolesAsync(user, oldRoles);

            var roleResult = await _userManager.AddToRoleAsync(user, role);

            if (!roleResult.Succeeded)
            {
                foreach (var error in roleResult.Errors)
                    ModelState.AddModelError("", error.Description);

                return View(new EditUserViewModel
                {
                    Id = id,
                    Email = email,
                    Role = role
                });
            }

            await _auditLogService.LogAsync(
                "Updated",
                "User",
                $"User '{oldEmail}' was updated to '{email}'. Role changed from '{oldRoleText}' to '{role}'."
            );

            TempData["Success"] = "User updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // Delete POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            if (User.Identity?.IsAuthenticated == true &&
                user.UserName == User.Identity.Name)
            {
                TempData["Error"] = "You cannot delete your own account.";
                return RedirectToAction(nameof(Index));
            }

            var deletedEmail = user.Email ?? "";
            var deletedRoles = await _userManager.GetRolesAsync(user);

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                await _auditLogService.LogAsync(
                    "Deleted",
                    "User",
                    $"User '{deletedEmail}' with role '{string.Join(", ", deletedRoles)}' was deleted."
                );

                TempData["Success"] = "User deleted successfully!";
            }
            else
            {
                TempData["Error"] = "Unable to delete user.";
            }

            return RedirectToAction(nameof(Index));
        }
    }

    public class UserViewModel
    {
        public string Id { get; set; } = "";
        public string Email { get; set; } = "";
        public string UserName { get; set; } = "";
        public string Roles { get; set; } = "";
    }

    public class EditUserViewModel
    {
        public string Id { get; set; } = "";
        public string Email { get; set; } = "";
        public string Role { get; set; } = "Employee";
    }
}
