using InventoryManagement.Helpers;
using InventoryManagement.Services.Interfaces;
using InventoryManagement.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace InventoryManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly SignInManager<Models.ApplicationUser> _singInManager;
        private readonly UserManager<Models.ApplicationUser> _userManager;
        public AccountController(IAccountService accountService, SignInManager<Models.ApplicationUser> signInManager, UserManager<Models.ApplicationUser> userManager)
        {
            _accountService = accountService;
            _singInManager = signInManager;
            _userManager = userManager;
        }
        public IActionResult Register() => View();
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await _accountService.RegisterAsync(model);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                await _singInManager.SignInAsync(user, isPersistent: true);
                return RedirectToAction(nameof(Register));
            }

            return View(model);
        }
        // GET: Login View
        public IActionResult Login() => View();
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await _accountService.LoginAsync(model);
            if (user != null)
            {
                var result = await _singInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Admin")) return RedirectToAction(nameof(AdminController.Dashboard),ControllerHelper.GetControllerName<AdminController>());
                if (roles.Contains("Manager")) return RedirectToAction(nameof(ManagerController.Dashboard),ControllerHelper.GetControllerName<ManagerController>());
                if (roles.Contains("Staff")) return RedirectToAction(nameof(StaffController.Dashboard),ControllerHelper.GetControllerName<StaffController>());


            }
            ModelState.AddModelError("", "Invalid login attempt");
            return View(model);
        }
        public async Task<IActionResult> Logout()
        {
            await _singInManager.SignOutAsync();
            return RedirectToAction();
        }
        
    
    }

}