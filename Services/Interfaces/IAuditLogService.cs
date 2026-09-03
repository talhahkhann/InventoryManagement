using InventoryManagement.Models;

namespace InventoryManagement.Services.Interfaces
{
    public interface IAuditLogService
    {
        /// <summary>Write a single audit entry.</summary>
        Task LogAsync(
            string performedBy,
            string role,
            string entityType,
            string action,
            string? entityId = null,
            string? details  = null);

        Task<(List<AuditLog> Items, int TotalCount)> GetPagedAsync(
            int page, int pageSize,
            string? entityType  = null,
            string? performedBy = null,
            DateTime? from      = null,
            DateTime? to        = null);

        Task<List<string>> GetDistinctEntityTypesAsync();
        Task<List<string>> GetDistinctUsersAsync();
    }
}
