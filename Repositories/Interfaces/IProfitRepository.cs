using InventoryManagement.Models;
using InventoryManagement.ViewModels;

namespace InventoryManagement.Repositories.Interfaces
{
    public interface IProfitRepository
    {
        // ── Write ─────────────────────────────────────────────────────────────
        Task InsertRangeAsync(IEnumerable<ProfitRecord> records);
        Task DeleteByInvoiceIdAsync(int invoiceId);

        // ── Read — aggregate queries ──────────────────────────────────────────
        Task<ProfitSummaryViewModel>             GetSummaryAsync(DateTime from, DateTime to);
        Task<List<ProfitByMonthViewModel>>       GetByMonthAsync(int year);
        Task<List<ProfitByYearViewModel>>        GetByYearAsync();
        Task<List<ProfitByProductViewModel>>     GetByProductAsync(DateTime from, DateTime to);
        Task<List<ProfitByCustomerViewModel>>    GetByCustomerAsync(DateTime from, DateTime to);
        Task<List<ProfitByCategoryViewModel>>    GetByCategoryAsync(DateTime from, DateTime to);
        Task<List<int>>                          GetAvailableYearsAsync();
    }
}
