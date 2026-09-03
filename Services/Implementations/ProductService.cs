using InventoryManagement.Models;
using InventoryManagement.Repositories.Interfaces;
using InventoryManagement.Services.Interfaces;

namespace InventoryManagement.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepo;

        public ProductService(IProductRepository productRepository)
        {
            _productRepo = productRepository;
        }

        public Task<IEnumerable<Product>> GetAllProductsAsync() => _productRepo.GetAllAsync();
        public Task<Product>             GetProductByIdAsync(int id) => _productRepo.GetByIdAsync(id);
        public Task                      AddProductAsync(Product product) => _productRepo.AddAsync(product);
        public Task                      UpdateProductAsync(Product product) => _productRepo.UpdateAsync(product);
        public Task                      DeleteProductAsync(int id) => _productRepo.DeleteAsync(id);

        public Task<Product> DeductStockAsync(int productId, int qty)
            => _productRepo.DeductStockAsync(productId, qty);

        public Task<Product> RestoreStockAsync(int productId, int qty)
            => _productRepo.RestoreStockAsync(productId, qty);
    }
}
