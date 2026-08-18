// Services/Interfaces/ICustomerProductPriceService.cs
public interface ICustomerProductPriceService
{
    Task<decimal> GetEffectiveRateAsync(int customerId, int productId, decimal defaultRate);
    Task UpsertRateAsync(int customerId, int productId, decimal rate);
    Task<decimal?> GetCustomRateOrNullAsync(int customerId, int productId);
}