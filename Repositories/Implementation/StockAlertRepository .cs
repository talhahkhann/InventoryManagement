using InventoryManagement.Data;
using InventoryManagement.Models;
using InventoryManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Repositories
{
    public class StockAlertRepository : IStockAlertRepository
    {
        private readonly ApplicationDbContext _context;

        public StockAlertRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StockAlert>> GetActiveAlertsAsync()
        {
            return await _context.StockAlerts
                .Include(a => a.Product)
                .Where(a => !a.IsResolved)
                .OrderByDescending(a => a.Severity)
                .ThenByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetActiveAlertCountAsync()
        {
            return await _context.StockAlerts
                .CountAsync(a => !a.IsResolved);
        }

        public async Task<StockAlert> GetAlertByIdAsync(int id)
        {
            return await _context.StockAlerts
                .Include(a => a.Product)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<StockAlert>> GetAlertsByProductIdAsync(int productId)
        {
            return await _context.StockAlerts
                .Where(a => a.ProductId == productId)
                .ToListAsync();
        }

        public async Task<IEnumerable<StockAlert>> GetUnresolvedAlertsByProductIdAsync(int productId)
        {
            return await _context.StockAlerts
                .Where(a => a.ProductId == productId && !a.IsResolved)
                .ToListAsync();
        }

        public async Task AddAsync(StockAlert alert)
        {
            await _context.StockAlerts.AddAsync(alert);
        }

        public async Task UpdateAsync(StockAlert alert)
        {
            _context.StockAlerts.Update(alert);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var alert = await _context.StockAlerts.FindAsync(id);
            if (alert != null)
            {
                _context.StockAlerts.Remove(alert);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<StockAlert>> GetAlertHistoryAsync(int take = 100)
        {
            return await _context.StockAlerts
                .Include(a => a.Product)
                .OrderByDescending(a => a.CreatedAt)
                .Take(take)
                .ToListAsync();
        }
    }
}