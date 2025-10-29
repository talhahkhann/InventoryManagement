// Repositories/Implementations/InvoiceRepository.cs
using InventoryManagement.Data;
using InventoryManagement.Models;
using Microsoft.EntityFrameworkCore;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly ApplicationDbContext _context;

    public InvoiceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Invoice>> GetAllInvoicesAsync()
    {
        return await _context.Invoices.Include(i => i.Customer)
                                      .Include(i => i.Items)
                                      .ThenInclude(it => it.Product)
                                      .ToListAsync();
    }

    public async Task<Invoice> GetInvoiceByIdAsync(int id)
    {
        return await _context.Invoices.Include(i => i.Customer)
                                      .Include(i => i.Items)
                                      .ThenInclude(it => it.Product)
                                      .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task AddInvoiceAsync(Invoice invoice)
    {
        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteInvoiceAsync(int invoiceId)
    {
        var invoice = await _context.Invoices.FindAsync(invoiceId);
        if (invoice != null)
        {
            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();
        }
    }
    public async Task<Invoice> UpdateInvoiceAsync(Invoice invoice)
    {
        var existingInvoice = await _context.Invoices.Include(i => i.Items).FirstOrDefaultAsync(i => i.Id == invoice.Id);
        if (existingInvoice == null)
        {
            throw new InvalidOperationException("Invoice Not Found");
        }
        //Update the header 
        existingInvoice.CustomerId = invoice.CustomerId;
        existingInvoice.TotalAmount = invoice.TotalAmount;
        //Replace Items
        _context.InvoiceItems.RemoveRange(existingInvoice.Items);
        foreach (var newItem in invoice.Items)
        {
            newItem.InvoiceId = existingInvoice.Id;
            _context.InvoiceItems.Add(newItem);
        }
        await _context.SaveChangesAsync();
        return existingInvoice;
    }

}
