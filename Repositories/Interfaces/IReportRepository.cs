using InventoryManagement.ViewModels;

namespace InventoryManagement.Repositories
{
    public interface IReportRepository
    {
        // ── Filtered sales ────────────────────────────────────────────────
        Task<List<ProductSalesReportViewModel>>  GetSalesPerProductAsync(DateTime from, DateTime to);
        Task<List<CategorySalesReportViewModel>> GetSalesPerCategoryAsync(DateTime from, DateTime to);

        // ── Top N ─────────────────────────────────────────────────────────
        Task<List<TopCustomerViewModel>> GetTopCustomersAsync(DateTime from, DateTime to, int top = 20);
        Task<List<TopProductViewModel>>  GetTopProductsAsync (DateTime from, DateTime to, int top = 20);

        // ── Available years (for year-picker shortcut) ────────────────────
        Task<List<int>> GetAvailableYearsAsync();
    }
}
