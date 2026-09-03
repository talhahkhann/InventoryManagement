using InventoryManagement.Models;

namespace InventoryManagement.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product> GetByIdAsync(int id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);

        /// <summary>
        /// Atomically deducts <paramref name="qty"/> from stock.
        /// Throws <see cref="InvalidOperationException"/> if stock would go negative.
        /// Returns the updated product (with new Quantity).
        /// </summary>
        Task<Product> DeductStockAsync(int productId, int qty);

        /// <summary>
        /// Atomically adds <paramref name="qty"/> back to stock (used on invoice delete / edit reversal).
        /// Returns the updated product.
        /// </summary>
        Task<Product> RestoreStockAsync(int productId, int qty);
    }
}
