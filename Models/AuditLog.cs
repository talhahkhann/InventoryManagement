using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Models
{
    /// <summary>
    /// Immutable record of every create / update / delete action across the system.
    /// Written by AuditService — never updated, never deleted through the application.
    /// </summary>
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }

        // ── Who ──────────────────────────────────────────────────────────
        /// <summary>Email / username of the user who performed the action.</summary>
        [Required]
        [MaxLength(256)]
        public string PerformedBy { get; set; } = string.Empty;

        /// <summary>Role of the user at the time of the action.</summary>
        [MaxLength(50)]
        public string Role { get; set; } = string.Empty;

        // ── What ─────────────────────────────────────────────────────────
        /// <summary>Entity type affected, e.g. "Invoice", "Product", "Customer".</summary>
        [Required]
        [MaxLength(100)]
        public string EntityType { get; set; } = string.Empty;

        /// <summary>Primary key of the affected entity (as string to support non-int PKs).</summary>
        [MaxLength(100)]
        public string? EntityId { get; set; }

        /// <summary>Action performed: Created | Updated | Deleted | Received (PO) | Resolved (Alert).</summary>
        [Required]
        [MaxLength(50)]
        public string Action { get; set; } = string.Empty;

        /// <summary>Human-readable summary of the change.</summary>
        [MaxLength(1000)]
        public string? Details { get; set; }

        // ── When ─────────────────────────────────────────────────────────
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
