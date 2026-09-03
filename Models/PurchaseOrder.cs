using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagement.Models
{
    public enum PurchaseOrderStatus
    {
        Draft    = 0,   // created, not yet received
        Received = 1,   // stock has been added to inventory
        Cancelled = 2
    }

    public class PurchaseOrder
    {
        [Key]
        public int Id { get; set; }

        // ── Supplier ─────────────────────────────────────────────────
        [Required]
        public int SupplierId { get; set; }
        [ForeignKey(nameof(SupplierId))]
        public Supplier Supplier { get; set; } = null!;

        // ── Dates ─────────────────────────────────────────────────────
        [Required]
        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Received Date")]
        public DateTime? ReceivedDate { get; set; }

        // ── Status ────────────────────────────────────────────────────
        [Required]
        public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Draft;

        // ── Financials ────────────────────────────────────────────────
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Total Cost")]
        public decimal TotalCost { get; set; }

        // ── Notes ─────────────────────────────────────────────────────
        [MaxLength(500)]
        public string? Notes { get; set; }

        // ── Who received it ──────────────────────────────────────────
        [MaxLength(100)]
        public string? ReceivedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ── Line items ────────────────────────────────────────────────
        public ICollection<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>();
    }
}
