using InventoryManagement.Models;
using InventoryManagement.Repositories.Interfaces;
using InventoryManagement.Services.Interfaces;

namespace InventoryManagement.Services.Implementations
{
    public class AreaService : IAreaService
    {
        private readonly IAreaRepository _areaRepository;

        public AreaService(IAreaRepository areaRepository)
        {
            _areaRepository = areaRepository;
        }

        public async Task<IEnumerable<Area>> GetAllAreasAsync()
        {
            return await _areaRepository.GetAllAreasAsync();
        }

        public async Task<Area> GetAreaByIdAsync(int id)
        {
            return await _areaRepository.GetAreaByIdAsync(id);
        }

        public async Task AddAreaAsync(Area area)
        {
            await _areaRepository.AddAreaAsync(area);
        }

        public async Task UpdateAreaAsync(Area area)
        {
            await _areaRepository.UpdateAreaAsync(area);
        }

        public async Task DeleteAreaAsync(int id)
        {
            await _areaRepository.DeleteAreaAsync(id);
        }
    }
}
