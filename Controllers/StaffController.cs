using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Controllers
{
    [Authorize(Roles = "Staff")]
    public class StaffController : Controller
    {
        // Redirect straight to the unified dashboard
        public IActionResult Dashboard() =>
            RedirectToAction("Index", "Home");
    }
}
