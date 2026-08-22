using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagement.Models
{
    public enum AlertSeverity
    {
        Warning = 0,   // Stock below threshold but > 0
        Critical = 1   // Stock is 0 or severely low
    }

    public class StockAlert
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        [Required]
        public int CurrentStock { get; set; }

        [Required]
        public int Threshold { get; set; }

        [Required]
        public AlertSeverity Severity { get; set; }

        public bool IsResolved { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ResolvedAt { get; set; }
    }
}