using InventoryManagement.Models;
using InventoryManagement.Repositories.Interfaces;
using InventoryManagement.Services.Interfaces;

namespace InventoryManagement.Services.Implementations
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _repo;
        private readonly ILogger<AuditLogService> _logger;

        public AuditLogService(IAuditLogRepository repo, ILogger<AuditLogService> logger)
        {
            _repo   = repo;
            _logger = logger;
        }

        public async Task LogAsync(
            string performedBy,
            string role,
            string entityType,
            string action,
            string? entityId = null,
            string? details  = null)
        {
            try
            {
                await _repo.AddAsync(new AuditLog
                {
                    PerformedBy = performedBy,
                    Role        = role,
                    EntityType  = entityType,
                    Action      = action,
                    EntityId    = entityId,
                    Details     = details,
                    Timestamp   = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                // Audit failures must never break the main request
                _logger.LogError(ex, "Audit log write failed for {Action} on {Entity}",
                    action, entityType);
            }
        }

        public Task<(List<AuditLog> Items, int TotalCount)> GetPagedAsync(
            int page, int pageSize,
            string? entityType  = null,
            string? performedBy = null,
            DateTime? from      = null,
            DateTime? to        = null)
            => _repo.GetPagedAsync(page, pageSize, entityType, performedBy, from, to);

        public Task<List<string>> GetDistinctEntityTypesAsync()
            => _repo.GetDistinctEntityTypesAsync();

        public Task<List<string>> GetDistinctUsersAsync()
            => _repo.GetDistinctUsersAsync();
    }
}
