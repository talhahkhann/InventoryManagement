using InventoryManagement.Models;

namespace InventoryManagement.Repositories.Interfaces
{
    public interface IStockAlertRepository
    {
        Task<IEnumerable<StockAlert>> GetActiveAlertsAsync();
        Task<int> GetActiveAlertCountAsync();
        Task<StockAlert> GetAlertByIdAsync(int id);
        Task<IEnumerable<StockAlert>> GetAlertsByProductIdAsync(int productId);
        Task<IEnumerable<StockAlert>> GetUnresolvedAlertsByProductIdAsync(int productId);
        Task AddAsync(StockAlert alert);
        Task UpdateAsync(StockAlert alert);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();
        Task<IEnumerable<StockAlert>> GetAlertHistoryAsync(int take = 100);
    }
}