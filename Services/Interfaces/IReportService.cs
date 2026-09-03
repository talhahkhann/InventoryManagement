using InventoryManagement.ViewModels;

namespace InventoryManagement.Services
{
    public interface IReportService
    {
        Task<SalesReportPageViewModel<ProductSalesReportViewModel>>  GetSalesPerProductAsync(SalesReportFilterViewModel filter);
        Task<SalesReportPageViewModel<CategorySalesReportViewModel>> GetSalesPerCategoryAsync(SalesReportFilterViewModel filter);
        Task<SalesReportPageViewModel<TopCustomerViewModel>>         GetTopCustomersAsync(SalesReportFilterViewModel filter, int top = 20);
        Task<SalesReportPageViewModel<TopProductViewModel>>          GetTopProductsAsync(SalesReportFilterViewModel filter, int top = 20);
    }
}
