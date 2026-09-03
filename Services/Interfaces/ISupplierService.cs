using InventoryManagement.Models;

namespace InventoryManagement.Services.Interfaces
{
    public interface ISupplierService
    {
        Task<IEnumerable<Supplier>> GetAllAsync();
        Task<IEnumerable<Supplier>> GetActiveAsync();   // for dropdowns
        Task<Supplier?>             GetByIdAsync(int id);
        Task                        AddAsync(Supplier supplier);
        Task                        UpdateAsync(Supplier supplier);
        Task                        DeleteAsync(int id);
    }
}
