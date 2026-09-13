using InventoryManagement.Models;

namespace InventoryManagement.Repositories.Interfaces
{
    public interface IAuditLogRepository
    {
        Task AddAsync(AuditLog entry);

        /// <summary>Paged query with optional filters.</summary>
        Task<(List<AuditLog> Items, int TotalCount)> GetPagedAsync(
            int page, int pageSize,
            string? entityType   = null,
            string? performedBy  = null,
            DateTime? from       = null,
            DateTime? to         = null);

        Task<List<string>> GetDistinctEntityTypesAsync();
        Task<List<string>> GetDistinctUsersAsync();
    }
}
