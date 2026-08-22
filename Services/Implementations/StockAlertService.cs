using InventoryManagement.Models;
using InventoryManagement.Repositories.Interfaces;
using InventoryManagement.Services.Interfaces;

namespace InventoryManagement.Services
{
    public class StockAlertService : IStockAlertService
    {
        private readonly IStockAlertRepository _alertRepository;
        private readonly ILogger<StockAlertService> _logger;

        public StockAlertService(
            IStockAlertRepository alertRepository,
            ILogger<StockAlertService> logger)
        {
            _alertRepository = alertRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<StockAlert>> GetActiveAlertsAsync()
        {
            return await _alertRepository.GetActiveAlertsAsync();
        }

        public async Task<int> GetActiveAlertCountAsync()
        {
            return await _alertRepository.GetActiveAlertCountAsync();
        }

        public async Task<StockAlert> GetAlertByIdAsync(int id)
        {
            return await _alertRepository.GetAlertByIdAsync(id);
        }

        public async Task ResolveAlertAsync(int id)
        {
            var alert = await _alertRepository.GetAlertByIdAsync(id);
            if (alert != null && !alert.IsResolved)
            {
                alert.IsResolved = true;
                alert.ResolvedAt = DateTime.UtcNow;
                await _alertRepository.UpdateAsync(alert);
                await _alertRepository.SaveChangesAsync();
            }
        }

        public async Task CreateOrUpdateAlertAsync(int productId, int currentStock, int threshold)
        {
            if (currentStock >= threshold) return;

            var existingAlert = (await _alertRepository.GetUnresolvedAlertsByProductIdAsync(productId))
                .FirstOrDefault();

            var severity = currentStock == 0 ? AlertSeverity.Critical : AlertSeverity.Warning;

            if (existingAlert != null)
            {
                if (existingAlert.CurrentStock != currentStock || existingAlert.Severity != severity)
                {
                    existingAlert.CurrentStock = currentStock;
                    existingAlert.Severity = severity;
                    existingAlert.CreatedAt = DateTime.UtcNow;
                    await _alertRepository.UpdateAsync(existingAlert);
                    await _alertRepository.SaveChangesAsync();
                }
                return;
            }

            var alert = new StockAlert
            {
                ProductId = productId,
                CurrentStock = currentStock,
                Threshold = threshold,
                Severity = severity,
                IsResolved = false,
                CreatedAt = DateTime.UtcNow
            };

            await _alertRepository.AddAsync(alert);
            await _alertRepository.SaveChangesAsync();
        }

        public async Task AutoResolveAlertsAsync(int productId, int currentStock)
        {
            var alerts = await _alertRepository.GetUnresolvedAlertsByProductIdAsync(productId);
            var resolvedAny = false;

            foreach (var alert in alerts)
            {
                if (currentStock >= alert.Threshold)
                {
                    alert.IsResolved = true;
                    alert.ResolvedAt = DateTime.UtcNow;
                    await _alertRepository.UpdateAsync(alert);
                    resolvedAny = true;
                }
            }

            if (resolvedAny)
                await _alertRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<StockAlert>> GetAlertHistoryAsync()
        {
            return await _alertRepository.GetAlertHistoryAsync();
        }
    }
}