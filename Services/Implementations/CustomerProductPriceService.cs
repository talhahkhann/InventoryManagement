// Services/CustomerProductPriceService.cs
using InventoryManagement.Data;
using InventoryManagement.Models;
using Microsoft.EntityFrameworkCore;

public class CustomerProductPriceService : ICustomerProductPriceService
{
    private readonly ApplicationDbContext _context;

    public CustomerProductPriceService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<decimal> GetEffectiveRateAsync(int customerId, int productId, decimal defaultRate)
    {
        var existing = await _context.CustomerProductPrices
            .FirstOrDefaultAsync(x => x.CustomerId == customerId && x.ProductId == productId);

        // If this customer has bought this product before, return their last rate.
        // Otherwise fall back to the product's default rate.
        return existing?.Rate ?? defaultRate;
    }
    public async Task<decimal?> GetCustomRateOrNullAsync(int customerId, int productId)
{
    var existing = await _context.CustomerProductPrices
        .FirstOrDefaultAsync(x => x.CustomerId == customerId && x.ProductId == productId);
    return existing?.Rate;
}
    public async Task UpsertRateAsync(int customerId, int productId, decimal rate)
    {
        var existing = await _context.CustomerProductPrices
            .FirstOrDefaultAsync(x => x.CustomerId == customerId && x.ProductId == productId);

        if (existing != null)
        {
            existing.Rate = rate;
            existing.LastUpdated = DateTime.UtcNow;
        }
        else
        {
            _context.CustomerProductPrices.Add(new CustomerProductPrice
            {
                CustomerId = customerId,
                ProductId = productId,
                Rate = rate,
                LastUpdated = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();
    }
}