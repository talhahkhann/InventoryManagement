using InventoryManagement.Models;

namespace InventoryManagement.Services.Interfaces
{
    public interface IStockAlertService
    {
        Task<IEnumerable<StockAlert>> GetActiveAlertsAsync();
        Task<int> GetActiveAlertCountAsync();
        Task<StockAlert> GetAlertByIdAsync(int id);
        Task ResolveAlertAsync(int id);
        Task CreateOrUpdateAlertAsync(int productId, int currentStock, int threshold);
        Task AutoResolveAlertsAsync(int productId, int currentStock);
        Task<IEnumerable<StockAlert>> GetAlertHistoryAsync();
    }
}