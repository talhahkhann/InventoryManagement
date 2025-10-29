using InventoryManagement.Models;

public interface IInvoiceRepository
{
    Task<List<Invoice>> GetAllInvoicesAsync();
    Task<Invoice> GetInvoiceByIdAsync(int id);
    Task AddInvoiceAsync(Invoice invoice);
    Task DeleteInvoiceAsync(int invoiceId);
    Task<Invoice> UpdateInvoiceAsync(Invoice invoice);
}