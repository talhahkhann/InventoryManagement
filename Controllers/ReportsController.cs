using InventoryManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Controllers
{
    // Reports are for Admin and Manager only.
    [Authorize(Roles = "Admin,Manager")]
    public class ReportsController : Controller
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        public async Task<IActionResult> SalesPerProduct()
        {
            var data = await _reportService.GetSalesPerProductAsync();
            return View(data);
        }

        public async Task<IActionResult> SalesPerCategory()
        {
            var data = await _reportService.GetSalesPerCategoryAsync();
            return View(data);
        }
    }
}
