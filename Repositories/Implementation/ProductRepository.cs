using InventoryManagement.Data;
using InventoryManagement.Models;
using InventoryManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Repositories.Implementations
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
            => await _context.Products.Include(p => p.Category).ToListAsync();

        public async Task<Product> GetByIdAsync(int id)
            => await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);

        public async Task AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Atomically deducts qty from stock inside a single round-trip.
        /// Uses optimistic row-level update to prevent overselling.
        /// </summary>
        public async Task<Product> DeductStockAsync(int productId, int qty)
        {
            // Re-fetch with tracking so EF detects the change
            var product = await _context.Products.FindAsync(productId)
                          ?? throw new InvalidOperationException($"Product {productId} not found.");

            if (product.Quantity < qty)
                throw new InvalidOperationException(
                    $"Insufficient stock for '{product.Name}'. " +
                    $"Available: {product.Quantity}, Requested: {qty}.");

            product.Quantity -= qty;
            await _context.SaveChangesAsync();
            return product;
        }

        /// <summary>
        /// Restores qty back to stock (called when invoice is deleted or items are reduced on edit).
        /// </summary>
        public async Task<Product> RestoreStockAsync(int productId, int qty)
        {
            var product = await _context.Products.FindAsync(productId)
                          ?? throw new InvalidOperationException($"Product {productId} not found.");

            product.Quantity += qty;
            await _context.SaveChangesAsync();
            return product;
        }
    }
}
