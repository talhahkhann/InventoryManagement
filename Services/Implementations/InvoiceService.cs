// Services/Implementations/InvoiceService.cs
using InventoryManagement.Models;
using InventoryManagement.Repositories.Interfaces;
using InventoryManagement.Services.Interfaces;

public class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository  _invoiceRepo;
    private readonly IProductService     _productService;
    private readonly IStockAlertService  _alertService;

    public InvoiceService(
        IInvoiceRepository  invoiceRepository,
        IProductService     productService,
        IStockAlertService  alertService)
    {
        _invoiceRepo    = invoiceRepository;
        _productService = productService;
        _alertService   = alertService;
    }

    // ── Read ─────────────────────────────────────────────────────────────
    public Task<List<Invoice>>  GetAllInvoicesAsync()       => _invoiceRepo.GetAllInvoicesAsync();
    public Task<Invoice>        GetInvoiceByIdAsync(int id) => _invoiceRepo.GetInvoiceByIdAsync(id);

    // ── Create ────────────────────────────────────────────────────────────
    /// <summary>
    /// Saves the invoice, deducts stock for every line item, then fires
    /// stock alerts (or auto-resolves them) for each affected product.
    /// </summary>
    public async Task CreateInvoiceAsync(Invoice invoice)
    {
        invoice.TotalAmount = invoice.Items.Sum(i => i.Total);
        await _invoiceRepo.AddInvoiceAsync(invoice);

        // Deduct stock + check alerts for each item
        foreach (var item in invoice.Items)
        {
            var product = await _productService.DeductStockAsync(item.ProductId, item.Quantity);
            await CheckAndFireAlertAsync(product);
        }
    }

    // ── Update ────────────────────────────────────────────────────────────
    /// <summary>
    /// Replaces invoice items. Restores stock for the OLD items first,
    /// then deducts stock for the NEW items, then re-checks alerts.
    /// </summary>
    public async Task<Invoice> UpdateInvoiceAsync(Invoice invoice)
    {
        // Load existing items BEFORE the update so we can restore their stock
        var existing = await _invoiceRepo.GetInvoiceByIdAsync(invoice.Id);
        if (existing == null)
            throw new InvalidOperationException($"Invoice {invoice.Id} not found.");

        // 1. Restore stock for every OLD line item
        foreach (var oldItem in existing.Items)
            await _productService.RestoreStockAsync(oldItem.ProductId, oldItem.Quantity);

        // 2. Persist the updated invoice (replaces items)
        var updated = await _invoiceRepo.UpdateInvoiceAsync(invoice);

        // 3. Deduct stock for every NEW line item + check alerts
        foreach (var newItem in invoice.Items)
        {
            var product = await _productService.DeductStockAsync(newItem.ProductId, newItem.Quantity);
            await CheckAndFireAlertAsync(product);
        }

        return updated;
    }

    // ── Delete ────────────────────────────────────────────────────────────
    /// <summary>
    /// Deletes the invoice and restores stock for all line items,
    /// auto-resolving any alerts whose threshold is now satisfied.
    /// </summary>
    public async Task DeleteInvoiceAsync(int invoiceId)
    {
        if (invoiceId <= 0)
            throw new ArgumentException("Invalid invoice ID.", nameof(invoiceId));

        // Load items before deleting so we can restore stock
        var invoice = await _invoiceRepo.GetInvoiceByIdAsync(invoiceId);
        if (invoice == null) return;

        // Restore stock for each line item, then auto-resolve alerts
        foreach (var item in invoice.Items)
        {
            var product = await _productService.RestoreStockAsync(item.ProductId, item.Quantity);
            await _alertService.AutoResolveAlertsAsync(product.Id, product.Quantity);
        }

        await _invoiceRepo.DeleteInvoiceAsync(invoiceId);
    }

    // ── Private helpers ───────────────────────────────────────────────────
    /// <summary>
    /// After a stock deduction: if new stock is below threshold → create/update alert;
    /// if it's back above threshold → auto-resolve any existing alert.
    /// </summary>
    private async Task CheckAndFireAlertAsync(Product product)
    {
        if (product.Quantity < product.StockThreshold)
            await _alertService.CreateOrUpdateAlertAsync(
                product.Id, product.Quantity, product.StockThreshold);
        else
            await _alertService.AutoResolveAlertsAsync(product.Id, product.Quantity);
    }
}
