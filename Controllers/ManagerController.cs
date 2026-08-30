using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Controllers
{
    [Authorize(Roles = "Manager")]
    public class ManagerController : Controller
    {
        // Redirect straight to the unified dashboard
        public IActionResult Dashboard() =>
            RedirectToAction("Index", "Home");
    }
}
