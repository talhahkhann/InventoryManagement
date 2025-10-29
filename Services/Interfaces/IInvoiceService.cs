using InventoryManagement.Models;

public interface IInvoiceService
{
    Task<List<Invoice>> GetAllInvoicesAsync();
    Task<Invoice> GetInvoiceByIdAsync(int id);
    Task CreateInvoiceAsync(Invoice invoice);
    Task<Invoice> UpdateInvoiceAsync(Invoice invoice);
    Task DeleteInvoiceAsync(int invoiceId);
}