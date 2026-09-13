using InventoryManagement.Data;
using InventoryManagement.Models;
using InventoryManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Repositories.Implementations
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly ApplicationDbContext _context;

        public SupplierRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Supplier>> GetAllAsync()
            => await _context.Suppliers
                             .OrderBy(s => s.Name)
                             .ToListAsync();

        public async Task<IEnumerable<Supplier>> GetActiveAsync()
            => await _context.Suppliers
                             .Where(s => s.IsActive)
                             .OrderBy(s => s.Name)
                             .ToListAsync();

        public async Task<Supplier?> GetByIdAsync(int id)
            => await _context.Suppliers
                             .Include(s => s.PurchaseOrders)
                             .FirstOrDefaultAsync(s => s.Id == id);

        public async Task AddAsync(Supplier supplier)
        {
            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Supplier supplier)
        {
            _context.Suppliers.Update(supplier);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier != null)
            {
                _context.Suppliers.Remove(supplier);
                await _context.SaveChangesAsync();
            }
        }
    }
}
