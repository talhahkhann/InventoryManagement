using InventoryManagement.Data;
using InventoryManagement.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly ApplicationDbContext _context;
        public ReportRepository(ApplicationDbContext context) => _context = context;

        public async Task<List<ProductSalesReportViewModel>> GetSalesPerProductAsync()
        {
            return await _context.InvoiceItems
                .Include(i => i.Product)
                .GroupBy(i => i.Product.Name)
                .Select(g => new ProductSalesReportViewModel
                {
                    ProductName       = g.Key,
                    TotalQuantitySold = g.Sum(x => x.Quantity),
                    TotalRevenue      = g.Sum(x => x.Quantity * x.Price)
                })
                .ToListAsync();
        }

        public async Task<List<CategorySalesReportViewModel>> GetSalesPerCategoryAsync()
        {
            return await _context.InvoiceItems
                .Include(i => i.Product).ThenInclude(p => p.Category)
                .GroupBy(i => i.Product.Category.Name)
                .Select(g => new CategorySalesReportViewModel
                {
                    CategoryName      = g.Key,
                    TotalQuantitySold = g.Sum(x => x.Quantity),
                    TotalRevenue      = g.Sum(x => x.Quantity * x.Price)
                })
                .ToListAsync();
        }
    }
}
