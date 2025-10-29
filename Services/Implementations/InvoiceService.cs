// Services/Implementations/InvoiceService.cs
using InventoryManagement.Models;
using InventoryManagement.Repositories.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;

public class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _invoiceRepository;

    public InvoiceService(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<List<Invoice>> GetAllInvoicesAsync() => await _invoiceRepository.GetAllInvoicesAsync();
    public async Task<Invoice> GetInvoiceByIdAsync(int id) => await _invoiceRepository.GetInvoiceByIdAsync(id);

    public async Task CreateInvoiceAsync(Invoice invoice)
    {
        invoice.TotalAmount = invoice.Items.Sum(i => i.Total);
        await _invoiceRepository.AddInvoiceAsync(invoice);
    }
    public async Task DeleteInvoiceAsync(int invoiceId) {
        if (invoiceId <= 0)
        {
            throw new ArgumentException("Invalid invoice ID");
        }
        await _invoiceRepository.DeleteInvoiceAsync(invoiceId);
    }
    public async Task<Invoice> UpdateInvoiceAsync(Invoice invoice) => await _invoiceRepository.UpdateInvoiceAsync(invoice);
}
