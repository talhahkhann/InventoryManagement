using InventoryManagement.Models;
using InventoryManagement.Repositories.Interfaces;
using InventoryManagement.Services.Interfaces;
using InventoryManagement.ViewModels;

namespace InventoryManagement.Services.Implementations
{
    public class ProfitService : IProfitService
    {
        private readonly IProfitRepository _repo;

        public ProfitService(IProfitRepository repo)
        {
            _repo = repo;
        }

        // ── Write ─────────────────────────────────────────────────────────────

        public async Task RecordProfitAsync(Invoice invoice)
        {
            var records = BuildRecords(invoice);
            await _repo.InsertRangeAsync(records);
        }

        public async Task ReplaceProfitAsync(Invoice invoice)
        {
            await _repo.DeleteByInvoiceIdAsync(invoice.Id);
            var records = BuildRecords(invoice);
            if (records.Any())
                await _repo.InsertRangeAsync(records);
        }

        public Task DeleteProfitAsync(int invoiceId)
            => _repo.DeleteByInvoiceIdAsync(invoiceId);

        // ── Build ProfitRecord rows from a fully-loaded Invoice ───────────────

        private static List<ProfitRecord> BuildRecords(Invoice invoice)
        {
            if (invoice.Items == null || !invoice.Items.Any())
                return new List<ProfitRecord>();

            var date = invoice.InvoiceDate;
            var records = new List<ProfitRecord>();

            foreach (var item in invoice.Items)
            {
                var revenue = item.Price * item.Quantity;
                var cost    = item.CostPrice * item.Quantity;
                var profit  = revenue - cost;
                var margin  = revenue == 0 ? 0m
                            : Math.Round(profit / revenue * 100, 4);

                records.Add(new ProfitRecord
                {
                    InvoiceId    = invoice.Id,
                    InvoiceItemId= item.Id,
                    ProductId    = item.ProductId,
                    CustomerId   = invoice.CustomerId,
                    Year         = date.Year,
                    Month        = date.Month,
                    InvoiceDate  = date,

                    // Snapshots — survive renames
                    ProductName  = item.Product?.Name  ?? "Unknown",
                    CustomerName = invoice.Customer?.Name ?? "Unknown",
                    CategoryName = item.Product?.Category?.Name ?? "—",
                    AreaName     = invoice.Customer?.Area?.AreaName ?? "—",

                    Quantity     = item.Quantity,
                    SellingPrice = item.Price,
                    CostPrice    = item.CostPrice,
                    Revenue      = revenue,
                    Cost         = cost,
                    Profit       = profit,
                    MarginPct    = margin,
                    RecordedAt   = DateTime.UtcNow
                });
            }

            return records;
        }

        // ── Read ──────────────────────────────────────────────────────────────

        public Task<List<int>>                       GetAvailableYearsAsync()
            => _repo.GetAvailableYearsAsync();

        public Task<ProfitSummaryViewModel>          GetSummaryAsync(ProfitFilterViewModel f)
            => _repo.GetSummaryAsync(f.EffectiveFrom, f.EffectiveTo);

        public Task<List<ProfitByMonthViewModel>>    GetByMonthAsync(int year)
            => _repo.GetByMonthAsync(year);

        public Task<List<ProfitByYearViewModel>>     GetByYearAsync()
            => _repo.GetByYearAsync();

        public Task<List<ProfitByProductViewModel>>  GetByProductAsync(ProfitFilterViewModel f)
            => _repo.GetByProductAsync(f.EffectiveFrom, f.EffectiveTo);

        public Task<List<ProfitByCustomerViewModel>> GetByCustomerAsync(ProfitFilterViewModel f)
            => _repo.GetByCustomerAsync(f.EffectiveFrom, f.EffectiveTo);

        public Task<List<ProfitByCategoryViewModel>> GetByCategoryAsync(ProfitFilterViewModel f)
            => _repo.GetByCategoryAsync(f.EffectiveFrom, f.EffectiveTo);

        // ── Dashboard — all dimensions in parallel ────────────────────────────

        public async Task<ProfitDashboardViewModel> GetDashboardAsync(ProfitFilterViewModel filter)
        {
            var years = await _repo.GetAvailableYearsAsync();
            int selectedYear = filter.Year ?? (years.Any() ? years.First() : DateTime.Today.Year);
            filter.Year ??= selectedYear;

            var prev = PrevFilter(filter);

            // Run all queries SEQUENTIALLY — EF Core DbContext is not thread-safe.
            // Task.WhenAll on the same scoped DbContext causes concurrent operation exceptions.
            var summary   = await _repo.GetSummaryAsync(filter.EffectiveFrom, filter.EffectiveTo);
            var prevSum   = await _repo.GetSummaryAsync(prev.EffectiveFrom,   prev.EffectiveTo);
            var byMonth   = await _repo.GetByMonthAsync(selectedYear);
            var byProduct = await _repo.GetByProductAsync(filter.EffectiveFrom,  filter.EffectiveTo);
            var byCustomer= await _repo.GetByCustomerAsync(filter.EffectiveFrom, filter.EffectiveTo);
            var byCategory= await _repo.GetByCategoryAsync(filter.EffectiveFrom, filter.EffectiveTo);
            var byYear    = await _repo.GetByYearAsync();

            summary.PrevRevenue      = prevSum.TotalRevenue;
            summary.PrevProfit       = prevSum.TotalProfit;
            summary.Filter           = filter;
            summary.AvailableYears   = years;
            summary.MonthlyBreakdown = byMonth;
            summary.TopProducts      = byProduct.Take(5).ToList();

            return new ProfitDashboardViewModel
            {
                Summary        = summary,
                ByMonth        = byMonth,
                ByYear         = byYear,
                ByProduct      = byProduct,
                ByCustomer     = byCustomer,
                ByCategory     = byCategory,
                Filter         = filter,
                AvailableYears = years
            };
        }

        private static ProfitFilterViewModel PrevFilter(ProfitFilterViewModel f)
        {
            if (f.Year.HasValue && f.Month.HasValue)
            {
                var d = new DateTime(f.Year.Value, f.Month.Value, 1).AddMonths(-1);
                return new ProfitFilterViewModel { Year = d.Year, Month = d.Month };
            }
            if (f.Year.HasValue)
                return new ProfitFilterViewModel { Year = f.Year.Value - 1 };

            var span = f.EffectiveTo - f.EffectiveFrom;
            return new ProfitFilterViewModel
            {
                From = f.EffectiveFrom - span - TimeSpan.FromTicks(1),
                To   = f.EffectiveFrom - TimeSpan.FromTicks(1)
            };
        }
    }
}
