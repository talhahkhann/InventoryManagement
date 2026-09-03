using InventoryManagement.Data;
using InventoryManagement.Models;
using InventoryManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Repositories.Implementations
{
    public class PurchaseOrderRepository : IPurchaseOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public PurchaseOrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PurchaseOrder>> GetAllAsync()
            => await _context.PurchaseOrders
                             .Include(po => po.Supplier)
                             .Include(po => po.Items)
                             .OrderByDescending(po => po.OrderDate)
                             .ToListAsync();

        public async Task<PurchaseOrder?> GetByIdAsync(int id)
            => await _context.PurchaseOrders
                             .Include(po => po.Supplier)
                             .Include(po => po.Items)
                                 .ThenInclude(i => i.Product)
                                     .ThenInclude(p => p.Category)
                             .FirstOrDefaultAsync(po => po.Id == id);

        public async Task<PurchaseOrder> CreateAsync(PurchaseOrder po)
        {
            _context.PurchaseOrders.Add(po);
            await _context.SaveChangesAsync();
            return po;
        }

        public async Task UpdateAsync(PurchaseOrder po)
        {
            _context.PurchaseOrders.Update(po);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var po = await _context.PurchaseOrders
                                   .Include(p => p.Items)
                                   .FirstOrDefaultAsync(p => p.Id == id);
            if (po != null)
            {
                _context.PurchaseOrderItems.RemoveRange(po.Items);
                _context.PurchaseOrders.Remove(po);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<PurchaseOrder>> GetBySupplierAsync(int supplierId)
            => await _context.PurchaseOrders
                             .Include(po => po.Items)
                             .Where(po => po.SupplierId == supplierId)
                             .OrderByDescending(po => po.OrderDate)
                             .ToListAsync();
    }
}
