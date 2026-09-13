using InventoryManagement.Repositories;
using InventoryManagement.ViewModels;

namespace InventoryManagement.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _repo;
        public ReportService(IReportRepository repo) => _repo = repo;

        public async Task<SalesReportPageViewModel<ProductSalesReportViewModel>> GetSalesPerProductAsync(
            SalesReportFilterViewModel filter)
        {
            var data  = await _repo.GetSalesPerProductAsync(filter.EffectiveFrom, filter.EffectiveTo);
            var years = await _repo.GetAvailableYearsAsync();
            return new SalesReportPageViewModel<ProductSalesReportViewModel>
            {
                Filter         = filter,
                Data           = data,
                AvailableYears = years
            };
        }

        public async Task<SalesReportPageViewModel<CategorySalesReportViewModel>> GetSalesPerCategoryAsync(
            SalesReportFilterViewModel filter)
        {
            var data  = await _repo.GetSalesPerCategoryAsync(filter.EffectiveFrom, filter.EffectiveTo);
            var years = await _repo.GetAvailableYearsAsync();
            return new SalesReportPageViewModel<CategorySalesReportViewModel>
            {
                Filter         = filter,
                Data           = data,
                AvailableYears = years
            };
        }

        public async Task<SalesReportPageViewModel<TopCustomerViewModel>> GetTopCustomersAsync(
            SalesReportFilterViewModel filter, int top = 20)
        {
            var data  = await _repo.GetTopCustomersAsync(filter.EffectiveFrom, filter.EffectiveTo, top);
            var years = await _repo.GetAvailableYearsAsync();
            return new SalesReportPageViewModel<TopCustomerViewModel>
            {
                Filter         = filter,
                Data           = data,
                AvailableYears = years
            };
        }

        public async Task<SalesReportPageViewModel<TopProductViewModel>> GetTopProductsAsync(
            SalesReportFilterViewModel filter, int top = 20)
        {
            var data  = await _repo.GetTopProductsAsync(filter.EffectiveFrom, filter.EffectiveTo, top);
            var years = await _repo.GetAvailableYearsAsync();
            return new SalesReportPageViewModel<TopProductViewModel>
            {
                Filter         = filter,
                Data           = data,
                AvailableYears = years
            };
        }
    }
}
