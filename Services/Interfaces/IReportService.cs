using InventoryManagement.ViewModels;

namespace InventoryManagement.Services
{
    public interface IReportService
    {
        Task<List<ProductSalesReportViewModel>> GetSalesPerProductAsync();
        Task<List<CategorySalesReportViewModel>> GetSalesPerCategoryAsync();
    }
}
