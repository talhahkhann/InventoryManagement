using InventoryManagement.Data;
using InventoryManagement.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly ApplicationDbContext _context;
        public ReportRepository(ApplicationDbContext context) => _context = context;

        // ── Sales per product (date-filtered) ────────────────────────────
        public async Task<List<ProductSalesReportViewModel>> GetSalesPerProductAsync(
            DateTime from, DateTime to)
        {
            var rows = await _context.InvoiceItems
                .Include(i => i.Invoice)
                .Include(i => i.Product).ThenInclude(p => p.Category)
                .Where(i => i.Invoice.InvoiceDate >= from && i.Invoice.InvoiceDate <= to)
                .GroupBy(i => new { i.Product.Name, CategoryName = i.Product.Category.Name })
                .Select(g => new
                {
                    g.Key.Name,
                    g.Key.CategoryName,
                    TotalQty     = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.Quantity * x.Price),
                    TotalUnits   = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(g => g.TotalRevenue)
                .ToListAsync();

            int rank = 1;
            return rows.Select(r => new ProductSalesReportViewModel
            {
                Rank              = rank++,
                ProductName       = r.Name,
                CategoryName      = r.CategoryName,
                TotalQuantitySold = r.TotalQty,
                TotalRevenue      = r.TotalRevenue,
                AvgUnitPrice      = r.TotalUnits > 0
                                    ? r.TotalRevenue / r.TotalUnits
                                    : 0m
            }).ToList();
        }

        // ── Sales per category (date-filtered) ───────────────────────────
        public async Task<List<CategorySalesReportViewModel>> GetSalesPerCategoryAsync(
            DateTime from, DateTime to)
        {
            var rows = await _context.InvoiceItems
                .Include(i => i.Invoice)
                .Include(i => i.Product).ThenInclude(p => p.Category)
                .Where(i => i.Invoice.InvoiceDate >= from && i.Invoice.InvoiceDate <= to)
                .GroupBy(i => i.Product.Category.Name)
                .Select(g => new
                {
                    CategoryName   = g.Key,
                    TotalQty       = g.Sum(x => x.Quantity),
                    TotalRevenue   = g.Sum(x => x.Quantity * x.Price),
                    ProductCount   = g.Select(x => x.ProductId).Distinct().Count()
                })
                .OrderByDescending(g => g.TotalRevenue)
                .ToListAsync();

            int rank = 1;
            return rows.Select(r => new CategorySalesReportViewModel
            {
                Rank              = rank++,
                CategoryName      = r.CategoryName,
                TotalQuantitySold = r.TotalQty,
                TotalRevenue      = r.TotalRevenue,
                ProductCount      = r.ProductCount
            }).ToList();
        }

        // ── Top customers ────────────────────────────────────────────────
        public async Task<List<TopCustomerViewModel>> GetTopCustomersAsync(
            DateTime from, DateTime to, int top = 20)
        {
            var rows = await _context.Invoices
                .Include(i => i.Customer).ThenInclude(c => c.Area)
                .Where(i => i.InvoiceDate >= from && i.InvoiceDate <= to)
                .GroupBy(i => new
                {
                    i.CustomerId,
                    CustomerName = i.Customer.Name,
                    AreaName     = i.Customer.Area.AreaName
                })
                .Select(g => new
                {
                    g.Key.CustomerId,
                    g.Key.CustomerName,
                    g.Key.AreaName,
                    InvoiceCount  = g.Count(),
                    TotalSpend    = g.Sum(x => x.TotalAmount),
                    LastPurchase  = g.Max(x => (DateTime?)x.InvoiceDate)
                })
                .OrderByDescending(g => g.TotalSpend)
                .Take(top)
                .ToListAsync();

            int rank = 1;
            return rows.Select(r => new TopCustomerViewModel
            {
                Rank          = rank++,
                CustomerId    = r.CustomerId,
                CustomerName  = r.CustomerName,
                AreaName      = r.AreaName,
                InvoiceCount  = r.InvoiceCount,
                TotalSpend    = r.TotalSpend,
                AvgOrderValue = r.InvoiceCount > 0 ? r.TotalSpend / r.InvoiceCount : 0m,
                LastPurchase  = r.LastPurchase
            }).ToList();
        }

        // ── Top products ─────────────────────────────────────────────────
        public async Task<List<TopProductViewModel>> GetTopProductsAsync(
            DateTime from, DateTime to, int top = 20)
        {
            var rows = await _context.InvoiceItems
                .Include(i => i.Invoice)
                .Include(i => i.Product).ThenInclude(p => p.Category)
                .Where(i => i.Invoice.InvoiceDate >= from && i.Invoice.InvoiceDate <= to)
                .GroupBy(i => new
                {
                    i.ProductId,
                    ProductName  = i.Product.Name,
                    CategoryName = i.Product.Category.Name
                })
                .Select(g => new
                {
                    g.Key.ProductId,
                    g.Key.ProductName,
                    g.Key.CategoryName,
                    TotalQty     = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.Quantity * x.Price),
                    TotalCost    = g.Sum(x => x.Quantity * x.CostPrice)
                })
                .OrderByDescending(g => g.TotalRevenue)
                .Take(top)
                .ToListAsync();

            int rank = 1;
            return rows.Select(r =>
            {
                decimal profit    = r.TotalRevenue - r.TotalCost;
                decimal marginPct = r.TotalRevenue > 0
                                    ? profit / r.TotalRevenue * 100m : 0m;
                return new TopProductViewModel
                {
                    Rank              = rank++,
                    ProductId         = r.ProductId,
                    ProductName       = r.ProductName,
                    CategoryName      = r.CategoryName,
                    TotalQuantitySold = r.TotalQty,
                    TotalRevenue      = r.TotalRevenue,
                    TotalProfit       = profit,
                    MarginPct         = marginPct
                };
            }).ToList();
        }

        // ── Available years ───────────────────────────────────────────────
        public async Task<List<int>> GetAvailableYearsAsync()
        {
            return await _context.Invoices
                .Select(i => i.InvoiceDate.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();
        }
    }
}
