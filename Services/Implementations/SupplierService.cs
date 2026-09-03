using InventoryManagement.Models;
using InventoryManagement.Repositories.Interfaces;
using InventoryManagement.Services.Interfaces;

namespace InventoryManagement.Services.Implementations
{
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _repo;

        public SupplierService(ISupplierRepository repo) => _repo = repo;

        public Task<IEnumerable<Supplier>> GetAllAsync()    => _repo.GetAllAsync();
        public Task<IEnumerable<Supplier>> GetActiveAsync() => _repo.GetActiveAsync();
        public Task<Supplier?>             GetByIdAsync(int id) => _repo.GetByIdAsync(id);
        public Task                        AddAsync(Supplier supplier) => _repo.AddAsync(supplier);
        public Task                        UpdateAsync(Supplier supplier) => _repo.UpdateAsync(supplier);
        public Task                        DeleteAsync(int id) => _repo.DeleteAsync(id);
    }
}
