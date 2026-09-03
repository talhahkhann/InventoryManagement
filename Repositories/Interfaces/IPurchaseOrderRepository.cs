using InventoryManagement.Models;

namespace InventoryManagement.Repositories.Interfaces
{
    public interface IPurchaseOrderRepository
    {
        Task<IEnumerable<PurchaseOrder>> GetAllAsync();
        Task<PurchaseOrder?>             GetByIdAsync(int id);        // includes Supplier + Items + Products
        Task<PurchaseOrder>              CreateAsync(PurchaseOrder po);
        Task                             UpdateAsync(PurchaseOrder po);
        Task                             DeleteAsync(int id);
        Task<IEnumerable<PurchaseOrder>> GetBySupplierAsync(int supplierId);
    }
}
