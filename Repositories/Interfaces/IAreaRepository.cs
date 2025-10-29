using InventoryManagement.Models;

namespace InventoryManagement.Repositories.Interfaces
{
    public interface IAreaRepository
    {
        Task<IEnumerable<Area>> GetAllAreasAsync();
        Task<Area> GetAreaByIdAsync(int id);
        Task AddAreaAsync(Area area);
        Task UpdateAreaAsync(Area area);
        Task DeleteAreaAsync(int id);
    }
}
