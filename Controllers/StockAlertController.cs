using InventoryManagement.Services.Interfaces;
using InventoryManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Controllers
{
    public class StockAlertController : Controller
    {
        private readonly IStockAlertService _alertService;
        private readonly ILogger<StockAlertController> _logger;

        public StockAlertController(
            IStockAlertService alertService,
            ILogger<StockAlertController> logger)
        {
            _alertService = alertService;
            _logger = logger;
        }

        // GET: StockAlert
        public async Task<IActionResult> Index()
        {
            var alerts = await _alertService.GetActiveAlertsAsync();
            var viewModels = alerts.Select(a => new StockAlertViewModel
            {
                Id = a.Id,
                ProductId = a.ProductId,
                ProductName = a.Product?.Name ?? "Unknown",
                CurrentStock = a.CurrentStock,
                Threshold = a.Threshold,
                Severity = a.Severity.ToString(),
                IsResolved = a.IsResolved,
                CreatedAt = a.CreatedAt,
                ResolvedAt = a.ResolvedAt
            }).ToList();

            return View(viewModels);
        }

        // GET: StockAlert/GetActiveAlerts (JSON for AJAX)
        [HttpGet]
        public async Task<IActionResult> GetActiveAlerts()
        {
            var alerts = await _alertService.GetActiveAlertsAsync();
            var viewModels = alerts.Select(a => new StockAlertViewModel
            {
                Id = a.Id,
                ProductId = a.ProductId,
                ProductName = a.Product?.Name ?? "Unknown",
                CurrentStock = a.CurrentStock,
                Threshold = a.Threshold,
                Severity = a.Severity.ToString(),
                IsResolved = a.IsResolved,
                CreatedAt = a.CreatedAt
            }).ToList();

            return Json(new { 
                count = viewModels.Count, 
                alerts = viewModels 
            });
        }

        // POST: StockAlert/Resolve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Resolve(int id)
        {
            try
            {
                await _alertService.ResolveAlertAsync(id);
                return Json(new { success = true, message = "Alert resolved." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resolving alert {AlertId}", id);
                return Json(new { success = false, message = "Failed to resolve alert." });
            }
        }

        // GET: StockAlert/History
        public async Task<IActionResult> History()
        {
            var alerts = await _alertService.GetAlertHistoryAsync();
            var viewModels = alerts.Select(a => new StockAlertViewModel
            {
                Id = a.Id,
                ProductId = a.ProductId,
                ProductName = a.Product?.Name ?? "Unknown",
                CurrentStock = a.CurrentStock,
                Threshold = a.Threshold,
                Severity = a.Severity.ToString(),
                IsResolved = a.IsResolved,
                CreatedAt = a.CreatedAt,
                ResolvedAt = a.ResolvedAt
            }).ToList();

            return View(viewModels);
        }
    }
}