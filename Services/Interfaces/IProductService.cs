using InventoryManagement.Models;

namespace InventoryManagement.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(int id);
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);

        /// <summary>
        /// Deducts <paramref name="qty"/> from a product's stock.
        /// Throws if insufficient stock.
        /// Returns updated product.
        /// </summary>
        Task<Product> DeductStockAsync(int productId, int qty);

        /// <summary>
        /// Restores <paramref name="qty"/> to a product's stock (invoice reversal).
        /// Returns updated product.
        /// </summary>
        Task<Product> RestoreStockAsync(int productId, int qty);
    }
}
