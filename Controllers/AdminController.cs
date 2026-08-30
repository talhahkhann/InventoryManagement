using InventoryManagement.Models;
using InventoryManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole>   _roleManager;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole>   roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Dashboard → go straight to the real unified dashboard
        public IActionResult Dashboard() =>
            RedirectToAction("Index", "Home");

        // ── User List ────────────────────────────────────────────────────────
        public async Task<IActionResult> Users()
        {
            var users      = _userManager.Users.ToList();
            var viewModels = new List<UserListViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                viewModels.Add(new UserListViewModel
                {
                    Id          = user.Id,
                    FullName    = user.FullName    ?? string.Empty,
                    Email       = user.Email       ?? string.Empty,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    Country     = user.Country     ?? string.Empty,
                    Role        = roles.FirstOrDefault() ?? "No Role"
                });
            }

            if (TempData["Success"] is string msg) ViewBag.Success = msg;
            if (TempData["Error"]   is string err) ViewBag.Error   = err;
            return View(viewModels);
        }

        // ── Create User ──────────────────────────────────────────────────────
        public IActionResult CreateUser() => View(new CreateUserViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = new ApplicationUser
            {
                UserName       = model.Email,
                Email          = model.Email,
                FullName       = model.FullName,
                PhoneNumber    = model.PhoneNumber,
                Address        = model.Address,
                Country        = model.Country,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.Role);
                TempData["Success"] = $"User '{model.FullName}' created with role '{model.Role}'.";
                return RedirectToAction(nameof(Users));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        // ── Edit User ────────────────────────────────────────────────────────
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles       = await _userManager.GetRolesAsync(user);
            var currentRole = roles.FirstOrDefault() ?? string.Empty;

            return View(new EditUserViewModel
            {
                Id          = user.Id,
                FullName    = user.FullName    ?? string.Empty,
                Email       = user.Email       ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                Address     = user.Address,
                Country     = user.Country,
                Role        = currentRole
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return NotFound();

            user.FullName    = model.FullName;
            user.Email       = model.Email;
            user.UserName    = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.Address     = model.Address;
            user.Country     = model.Country;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var e in updateResult.Errors)
                    ModelState.AddModelError(string.Empty, e.Description);
                return View(model);
            }

            // Role swap
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (!currentRoles.Contains(model.Role))
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, model.Role);
            }

            // Optional password reset
            if (!string.IsNullOrWhiteSpace(model.NewPassword))
            {
                var token     = await _userManager.GeneratePasswordResetTokenAsync(user);
                var pwdResult = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
                if (!pwdResult.Succeeded)
                {
                    foreach (var e in pwdResult.Errors)
                        ModelState.AddModelError(string.Empty, e.Description);
                    return View(model);
                }
            }

            TempData["Success"] = $"User '{model.FullName}' updated successfully.";
            return RedirectToAction(nameof(Users));
        }

        // ── Delete User ──────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (id == currentUserId)
            {
                TempData["Error"] = "You cannot delete your own account.";
                return RedirectToAction(nameof(Users));
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var name   = user.FullName;
            var result = await _userManager.DeleteAsync(user);
            TempData[result.Succeeded ? "Success" : "Error"] =
                result.Succeeded ? $"User '{name}' deleted." : "Failed to delete user.";

            return RedirectToAction(nameof(Users));
        }
    }
}
