using InventoryManagement.Repositories;
using InventoryManagement.ViewModels;

namespace InventoryManagement.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _repo;
        public ReportService(IReportRepository repo) => _repo = repo;

        public Task<List<ProductSalesReportViewModel>>  GetSalesPerProductAsync()  => _repo.GetSalesPerProductAsync();
        public Task<List<CategorySalesReportViewModel>> GetSalesPerCategoryAsync() => _repo.GetSalesPerCategoryAsync();
    }
}
