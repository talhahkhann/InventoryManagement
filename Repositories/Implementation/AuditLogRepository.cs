using InventoryManagement.Data;
using InventoryManagement.Models;
using InventoryManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Repositories.Implementations
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly ApplicationDbContext _context;

        public AuditLogRepository(ApplicationDbContext context)
            => _context = context;

        public async Task AddAsync(AuditLog entry)
        {
            _context.AuditLogs.Add(entry);
            await _context.SaveChangesAsync();
        }

        public async Task<(List<AuditLog> Items, int TotalCount)> GetPagedAsync(
            int page, int pageSize,
            string? entityType  = null,
            string? performedBy = null,
            DateTime? from      = null,
            DateTime? to        = null)
        {
            var query = _context.AuditLogs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(entityType))
                query = query.Where(a => a.EntityType == entityType);

            if (!string.IsNullOrWhiteSpace(performedBy))
                query = query.Where(a => a.PerformedBy.Contains(performedBy));

            if (from.HasValue)
                query = query.Where(a => a.Timestamp >= from.Value);

            if (to.HasValue)
                query = query.Where(a => a.Timestamp <= to.Value.AddDays(1).AddSeconds(-1));

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(a => a.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public async Task<List<string>> GetDistinctEntityTypesAsync()
            => await _context.AuditLogs
                .Select(a => a.EntityType)
                .Distinct()
                .OrderBy(e => e)
                .ToListAsync();

        public async Task<List<string>> GetDistinctUsersAsync()
            => await _context.AuditLogs
                .Select(a => a.PerformedBy)
                .Distinct()
                .OrderBy(u => u)
                .ToListAsync();
    }
}
