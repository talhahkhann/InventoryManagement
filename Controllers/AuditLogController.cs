using InventoryManagement.Services.Interfaces;
using InventoryManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace InventoryManagement.Controllers
{
    // Audit log is Admin-only — no manager or staff
    [Authorize(Roles = "Admin")]
    public class AuditLogController : Controller
    {
        private readonly IAuditLogService _auditService;

        public AuditLogController(IAuditLogService auditService)
            => _auditService = auditService;

        // GET /AuditLog
        public async Task<IActionResult> Index(AuditLogFilterViewModel filter)
        {
            if (filter.Page < 1) filter.Page = 1;

            var (items, total) = await _auditService.GetPagedAsync(
                filter.Page, filter.PageSize,
                filter.EntityType, filter.PerformedBy,
                filter.From, filter.To);

            var rows = items.Select(a => new AuditLogRowViewModel
            {
                Id          = a.Id,
                PerformedBy = a.PerformedBy,
                Role        = a.Role,
                EntityType  = a.EntityType,
                EntityId    = a.EntityId,
                Action      = a.Action,
                Details     = a.Details,
                Timestamp   = a.Timestamp
            }).ToList();

            var vm = new AuditLogPageViewModel
            {
                Filter      = filter,
                Items       = rows,
                TotalCount  = total,
                EntityTypes = await _auditService.GetDistinctEntityTypesAsync(),
                Users       = await _auditService.GetDistinctUsersAsync()
            };

            return View(vm);
        }

        // GET /AuditLog/ExportCsv
        [HttpGet]
        public async Task<IActionResult> ExportCsv(AuditLogFilterViewModel filter)
        {
            // Export all (no page size limit)
            filter.Page     = 1;
            filter.PageSize = int.MaxValue;

            var (items, _) = await _auditService.GetPagedAsync(
                filter.Page, filter.PageSize,
                filter.EntityType, filter.PerformedBy,
                filter.From, filter.To);

            var sb = new StringBuilder();
            sb.AppendLine("Timestamp (UTC),Performed By,Role,Entity,Entity ID,Action,Details");
            foreach (var a in items)
            {
                var detail = (a.Details ?? "").Replace("\"", "\"\"");
                sb.AppendLine(
                    $"{a.Timestamp:yyyy-MM-dd HH:mm:ss}," +
                    $"\"{a.PerformedBy}\"," +
                    $"\"{a.Role}\"," +
                    $"\"{a.EntityType}\"," +
                    $"\"{a.EntityId ?? ""}\"," +
                    $"\"{a.Action}\"," +
                    $"\"{detail}\"");
            }

            var filename = $"audit_log_{DateTime.UtcNow:yyyyMMdd_HHmm}.csv";
            return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", filename);
        }
    }
}
