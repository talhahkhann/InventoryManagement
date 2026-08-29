using InventoryManagement.ViewModels;

namespace InventoryManagement.Repositories
{
    public interface IReportRepository
    {
        Task<List<ProductSalesReportViewModel>>  GetSalesPerProductAsync();
        Task<List<CategorySalesReportViewModel>> GetSalesPerCategoryAsync();
    }
}
