using InventoryManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Controllers
{
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
