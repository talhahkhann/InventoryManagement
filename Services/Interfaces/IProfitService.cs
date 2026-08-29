using InventoryManagement.Models;
using InventoryManagement.ViewModels;

namespace InventoryManagement.Services.Interfaces
{
    public interface IProfitService
    {
        // ── Write (called from InvoiceController) ─────────────────────────────
        Task RecordProfitAsync(Invoice invoice);          // create: insert rows
        Task ReplaceProfitAsync(Invoice invoice);         // edit:   delete + re-insert
        Task DeleteProfitAsync(int invoiceId);            // delete invoice

        // ── Read ─────────────────────────────────────────────────────────────
        Task<ProfitDashboardViewModel>           GetDashboardAsync(ProfitFilterViewModel filter);
        Task<ProfitSummaryViewModel>             GetSummaryAsync(ProfitFilterViewModel filter);
        Task<List<ProfitByMonthViewModel>>       GetByMonthAsync(int year);
        Task<List<ProfitByYearViewModel>>        GetByYearAsync();
        Task<List<ProfitByProductViewModel>>     GetByProductAsync(ProfitFilterViewModel filter);
        Task<List<ProfitByCustomerViewModel>>    GetByCustomerAsync(ProfitFilterViewModel filter);
        Task<List<ProfitByCategoryViewModel>>    GetByCategoryAsync(ProfitFilterViewModel filter);
        Task<List<int>>                          GetAvailableYearsAsync();
    }
}
