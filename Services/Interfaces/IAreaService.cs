using InventoryManagement.Models;

namespace InventoryManagement.Services.Interfaces
{
    public interface IAreaService
    {
        Task<IEnumerable<Area>> GetAllAreasAsync();
        Task<Area> GetAreaByIdAsync(int id);
        Task AddAreaAsync(Area area);
        Task UpdateAreaAsync(Area area);
        Task DeleteAreaAsync(int id);
    }
}
