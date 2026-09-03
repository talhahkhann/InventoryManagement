using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagement.Models
{
    public class PurchaseOrderItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PurchaseOrderId { get; set; }
        [ForeignKey(nameof(PurchaseOrderId))]
        public PurchaseOrder PurchaseOrder { get; set; } = null!;

        [Required]
        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; } = null!;

        [Required]
        [Range(1, 100000)]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        /// <summary>Unit cost paid to supplier for this order.</summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, 1000000)]
        [Display(Name = "Unit Cost (PKR)")]
        public decimal UnitCost { get; set; }

        /// <summary>Computed: UnitCost × Quantity</summary>
        [NotMapped]
        public decimal LineTotal => UnitCost * Quantity;
    }
}
