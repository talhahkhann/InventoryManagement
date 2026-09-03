using InventoryManagement.Models;
using InventoryManagement.ViewModels;

namespace InventoryManagement.Services.Interfaces
{
    public interface IPurchaseOrderService
    {
        Task<IEnumerable<PurchaseOrder>> GetAllAsync();
        Task<PurchaseOrder?>             GetByIdAsync(int id);

        /// <summary>Creates a Draft PO.</summary>
        Task<PurchaseOrder> CreateAsync(PurchaseOrderCreateViewModel vm, string createdByEmail);

        /// <summary>
        /// Marks PO as Received, adds stock for every line item,
        /// updates each product's CostPrice to the supplied unit cost,
        /// and fires / auto-resolves stock alerts.
        /// </summary>
        Task ReceiveOrderAsync(int purchaseOrderId, string receivedByEmail);

        /// <summary>Deletes a Draft PO. Received POs cannot be deleted.</summary>
        Task DeleteAsync(int id);
    }
}
