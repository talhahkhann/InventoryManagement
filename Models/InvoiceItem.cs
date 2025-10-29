using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Models
{
    public class InvoiceItem
    {
        public int Id { get; set; }

        [Required]
        public int InvoiceId { get; set; } // FK
        public Invoice Invoice { get; set; }

        [Required]
        public int ProductId { get; set; } // FK
        public Product Product { get; set; }

        public int Quantity { get; set; }
        public decimal Price { get; set; } // Price per unit
        public decimal Total => Price * Quantity; // Calculated
    }
}