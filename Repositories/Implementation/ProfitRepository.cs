using InventoryManagement.Data;
using InventoryManagement.Models;
using InventoryManagement.Repositories.Interfaces;
using InventoryManagement.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Repositories.Implementations
{
    public class ProfitRepository : IProfitRepository
    {
        private readonly ApplicationDbContext _context;

        public ProfitRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // ── Write ─────────────────────────────────────────────────────────────

        public async Task InsertRangeAsync(IEnumerable<ProfitRecord> records)
        {
            await _context.ProfitRecords.AddRangeAsync(records);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByInvoiceIdAsync(int invoiceId)
        {
            var rows = await _context.ProfitRecords
                .Where(r => r.InvoiceId == invoiceId)
                .ToListAsync();

            if (rows.Any())
            {
                _context.ProfitRecords.RemoveRange(rows);
                await _context.SaveChangesAsync();
            }
        }

        // ── Summary — single query, all aggregates at once ────────────────────

        public async Task<ProfitSummaryViewModel> GetSummaryAsync(DateTime from, DateTime to)
        {
            // Materialise once, then aggregate in memory — avoids 5 separate DB round-trips
            // and the concurrent-context error that would occur if they were awaited in parallel.
            var rows = await _context.ProfitRecords
                .Where(r => r.InvoiceDate >= from && r.InvoiceDate <= to)
                .Select(r => new { r.Revenue, r.Cost, r.Profit, r.Quantity, r.InvoiceId })
                .ToListAsync();

            return new ProfitSummaryViewModel
            {
                TotalRevenue   = rows.Sum(r => r.Revenue),
                TotalCost      = rows.Sum(r => r.Cost),
                TotalProfit    = rows.Sum(r => r.Profit),
                TotalUnitsSold = rows.Sum(r => r.Quantity),
                TotalInvoices  = rows.Select(r => r.InvoiceId).Distinct().Count()
            };
        }

        // ── By Month ──────────────────────────────────────────────────────────

        public async Task<List<ProfitByMonthViewModel>> GetByMonthAsync(int year)
        {
            return await _context.ProfitRecords
                .Where(r => r.Year == year)
                .GroupBy(r => new { r.Year, r.Month })
                .Select(g => new ProfitByMonthViewModel
                {
                    Year         = g.Key.Year,
                    Month        = g.Key.Month,
                    Revenue      = g.Sum(r => r.Revenue),
                    Cost         = g.Sum(r => r.Cost),
                    Profit       = g.Sum(r => r.Profit),
                    UnitsSold    = g.Sum(r => r.Quantity),
                    InvoiceCount = g.Select(r => r.InvoiceId).Distinct().Count()
                })
                .OrderBy(r => r.Month)
                .ToListAsync();
        }

        // ── By Year ───────────────────────────────────────────────────────────

        public async Task<List<ProfitByYearViewModel>> GetByYearAsync()
        {
            return await _context.ProfitRecords
                .GroupBy(r => r.Year)
                .Select(g => new ProfitByYearViewModel
                {
                    Year         = g.Key,
                    Revenue      = g.Sum(r => r.Revenue),
                    Cost         = g.Sum(r => r.Cost),
                    Profit       = g.Sum(r => r.Profit),
                    UnitsSold    = g.Sum(r => r.Quantity),
                    InvoiceCount = g.Select(r => r.InvoiceId).Distinct().Count()
                })
                .OrderByDescending(r => r.Year)
                .ToListAsync();
        }

        // ── By Product ────────────────────────────────────────────────────────

        public async Task<List<ProfitByProductViewModel>> GetByProductAsync(DateTime from, DateTime to)
        {
            return await _context.ProfitRecords
                .Where(r => r.InvoiceDate >= from && r.InvoiceDate <= to)
                .GroupBy(r => new { r.ProductId, r.ProductName, r.CategoryName })
                .Select(g => new ProfitByProductViewModel
                {
                    ProductId    = g.Key.ProductId,
                    ProductName  = g.Key.ProductName,
                    CategoryName = g.Key.CategoryName,
                    Revenue      = g.Sum(r => r.Revenue),
                    Cost         = g.Sum(r => r.Cost),
                    Profit       = g.Sum(r => r.Profit),
                    UnitsSold    = g.Sum(r => r.Quantity)
                })
                .OrderByDescending(r => r.Profit)
                .ToListAsync();
        }

        // ── By Customer ───────────────────────────────────────────────────────

        public async Task<List<ProfitByCustomerViewModel>> GetByCustomerAsync(DateTime from, DateTime to)
        {
            return await _context.ProfitRecords
                .Where(r => r.InvoiceDate >= from && r.InvoiceDate <= to)
                .GroupBy(r => new { r.CustomerId, r.CustomerName, r.AreaName })
                .Select(g => new ProfitByCustomerViewModel
                {
                    CustomerId   = g.Key.CustomerId,
                    CustomerName = g.Key.CustomerName,
                    AreaName     = g.Key.AreaName,
                    Revenue      = g.Sum(r => r.Revenue),
                    Cost         = g.Sum(r => r.Cost),
                    Profit       = g.Sum(r => r.Profit),
                    UnitsSold    = g.Sum(r => r.Quantity),
                    InvoiceCount = g.Select(r => r.InvoiceId).Distinct().Count()
                })
                .OrderByDescending(r => r.Profit)
                .ToListAsync();
        }

        // ── By Category ───────────────────────────────────────────────────────

        public async Task<List<ProfitByCategoryViewModel>> GetByCategoryAsync(DateTime from, DateTime to)
        {
            return await _context.ProfitRecords
                .Where(r => r.InvoiceDate >= from && r.InvoiceDate <= to)
                .GroupBy(r => r.CategoryName)
                .Select(g => new ProfitByCategoryViewModel
                {
                    CategoryName = g.Key,
                    Revenue      = g.Sum(r => r.Revenue),
                    Cost         = g.Sum(r => r.Cost),
                    Profit       = g.Sum(r => r.Profit),
                    UnitsSold    = g.Sum(r => r.Quantity)
                })
                .OrderByDescending(r => r.Profit)
                .ToListAsync();
        }

        // ── Available years ───────────────────────────────────────────────────

        public async Task<List<int>> GetAvailableYearsAsync()
        {
            var years = await _context.ProfitRecords
                .Select(r => r.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();

            // Always include current year even if no data yet
            if (!years.Contains(DateTime.Today.Year))
                years.Insert(0, DateTime.Today.Year);

            return years;
        }
    }
}
