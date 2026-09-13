    using InventoryManagement.Data;
    using InventoryManagement.ViewModels;
    using Microsoft.EntityFrameworkCore;

    namespace InventoryManagement.Repositories
    {
        public class ReportRepository : IReportRepository
        {
            private readonly ApplicationDbContext _context;

            public ReportRepository(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<List<ProductSalesReportViewModel>> GetSalesPerProductAsync(DateTime from, DateTime to)
            {
                return await _context.InvoiceItems
                    .Include(i => i.Product)
                    .Include(i => i.Invoice)
                    .Where(i => i.Invoice.InvoiceDate >= from && i.Invoice.InvoiceDate <= to)
                    .GroupBy(i => i.Product.Name)
                    .Select(g => new ProductSalesReportViewModel
                    {
                        ProductName = g.Key,
                        TotalQuantitySold = g.Sum(x => x.Quantity),
                        TotalRevenue = g.Sum(x => x.Quantity * x.Price)
                    })
                    .ToListAsync();
            }

            public async Task<List<CategorySalesReportViewModel>> GetSalesPerCategoryAsync(DateTime from, DateTime to)
            {
                return await _context.InvoiceItems
                    .Include(i => i.Product)
                        .ThenInclude(p => p.Category)
                    .Include(i => i.Invoice)
                    .Where(i => i.Invoice.InvoiceDate >= from && i.Invoice.InvoiceDate <= to)
                    .GroupBy(i => i.Product.Category.Name)
                    .Select(g => new CategorySalesReportViewModel
                    {
                        CategoryName = g.Key,
                        TotalQuantitySold = g.Sum(x => x.Quantity),
                        TotalRevenue = g.Sum(x => x.Quantity * x.Price)
                    })
                    .ToListAsync();
            }

            public async Task<List<TopCustomerViewModel>> GetTopCustomersAsync(DateTime from, DateTime to, int top = 20)
{
    var results = await _context.InvoiceItems
        .Include(i => i.Invoice)
            .ThenInclude(inv => inv.Customer) // assumes Invoice.Customer navigation
        .Where(i => i.Invoice.InvoiceDate >= from && i.Invoice.InvoiceDate <= to) // assumes Invoice.InvoiceDate
        .GroupBy(i => new
        {
            i.Invoice.Customer.Id,
            i.Invoice.Customer.Name,
            AreaName = i.Invoice.Customer.Area.AreaName // assumes Customer.Area navigation with Name
        })
        .Select(g => new TopCustomerViewModel
        {
            CustomerId    = g.Key.Id,
            CustomerName  = g.Key.Name,
            AreaName      = g.Key.AreaName,
            InvoiceCount  = g.Select(x => x.InvoiceId).Distinct().Count(),
            TotalSpend    = g.Sum(x => x.Quantity * x.Price),
            AvgOrderValue = g.Select(x => x.InvoiceId).Distinct().Count() > 0
                ? g.Sum(x => x.Quantity * x.Price) / g.Select(x => x.InvoiceId).Distinct().Count()
                : 0,
            LastPurchase  = g.Max(x => x.Invoice.InvoiceDate)
        })
        .OrderByDescending(c => c.TotalSpend)
        .Take(top)
        .ToListAsync();

    for (int i = 0; i < results.Count; i++)
        results[i].Rank = i + 1;

    return results;
}

            public async Task<List<TopProductViewModel>> GetTopProductsAsync(DateTime from, DateTime to, int top = 20)
            {
                return await _context.InvoiceItems
                    .Include(i => i.Product)
                    .Include(i => i.Invoice)
                    .Where(i => i.Invoice.InvoiceDate >= from && i.Invoice.InvoiceDate <= to)
                    .GroupBy(i => new { i.Product.Id, i.Product.Name })
                    .Select(g => new TopProductViewModel
                    {
                        ProductId = g.Key.Id,
                        ProductName = g.Key.Name,
                        TotalQuantitySold = g.Sum(x => x.Quantity),
                        TotalRevenue = g.Sum(x => x.Quantity * x.Price)
                    })
                    .OrderByDescending(p => p.TotalRevenue)
                    .Take(top)
                    .ToListAsync();
            }

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