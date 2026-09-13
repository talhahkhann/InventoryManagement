using InventoryManagement.Models;

namespace InventoryManagement.Repositories.Interfaces
{
    public interface ISupplierRepository
    {
        Task<IEnumerable<Supplier>> GetAllAsync();
        Task<IEnumerable<Supplier>> GetActiveAsync();
        Task<Supplier?>             GetByIdAsync(int id);
        Task                        AddAsync(Supplier supplier);
        Task                        UpdateAsync(Supplier supplier);
        Task                        DeleteAsync(int id);
    }
}
