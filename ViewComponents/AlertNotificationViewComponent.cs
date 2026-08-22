using InventoryManagement.Services.Interfaces;
using InventoryManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.ViewComponents
{
    public class AlertNotificationViewComponent : ViewComponent
    {
        private readonly IStockAlertService _alertService;

        public AlertNotificationViewComponent(IStockAlertService alertService)
        {
            _alertService = alertService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var alerts = await _alertService.GetActiveAlertsAsync();
            var recentAlerts = alerts.Take(5).Select(a => new StockAlertViewModel
            {
                Id = a.Id,
                ProductId = a.ProductId,
                ProductName = a.Product?.Name ?? "Unknown",
                CurrentStock = a.CurrentStock,
                Threshold = a.Threshold,
                Severity = a.Severity.ToString(),
                CreatedAt = a.CreatedAt
            }).ToList();

            var vm = new AlertNotificationViewModel
            {
                UnresolvedCount = alerts.Count(),
                RecentAlerts = recentAlerts
            };

            return View(vm);
        }
    }
}