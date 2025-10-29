using InventoryManagement.Repositories;
using InventoryManagement.ViewModels;

namespace InventoryManagement.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;

        public ReportService(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public Task<List<ProductSalesReportViewModel>> GetSalesPerProductAsync()
        {
            return _reportRepository.GetSalesPerProductAsync();
        }

        public Task<List<CategorySalesReportViewModel>> GetSalesPerCategoryAsync()
        {
            return _reportRepository.GetSalesPerCategoryAsync();
        }
    }
}
