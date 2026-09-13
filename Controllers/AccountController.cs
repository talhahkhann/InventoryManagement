using InventoryManagement.Services.Interfaces;
using InventoryManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly SignInManager<Models.ApplicationUser> _signInManager;
        private readonly UserManager<Models.ApplicationUser> _userManager;

        public AccountController(
            IAccountService accountService,
            SignInManager<Models.ApplicationUser> signInManager,
            UserManager<Models.ApplicationUser> userManager)
        {
            _accountService = accountService;
            _signInManager  = signInManager;
            _userManager    = userManager;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Registration — Admin only (Admin creates accounts for others)
        // ─────────────────────────────────────────────────────────────────────
        [Authorize(Roles = "Admin")]
        public IActionResult Register() => View();

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _accountService.RegisterAsync(model);
            if (result.Succeeded)
            {
                TempData["Success"] = $"User '{model.FullName}' ({model.Role}) created successfully.";
                return RedirectToAction("Users", "Admin");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Login
        // ─────────────────────────────────────────────────────────────────────
        [AllowAnonymous]
        public IActionResult Login()
        {
            // If already authenticated, redirect to appropriate dashboard
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToDashboard();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user  = await _userManager.FindByEmailAsync(model.Email);
                var roles = await _userManager.GetRolesAsync(user!);

                if (roles.Any())
                    return RedirectToAction("Index", "Home");

                // Authenticated but no role assigned
                ModelState.AddModelError(string.Empty,
                    "Your account has no role assigned. Contact an administrator.");
                await _signInManager.SignOutAsync();
                return View(model);
            }

            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(model);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Logout
        // ─────────────────────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        // ─────────────────────────────────────────────────────────────────────
        // Access Denied
        // ─────────────────────────────────────────────────────────────────────
        [AllowAnonymous]
        public IActionResult AccessDenied() => View();

        // ─────────────────────────────────────────────────────────────────────
        private IActionResult RedirectToDashboard() =>
            RedirectToAction("Index", "Home");
    }
}
