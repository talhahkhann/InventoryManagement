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
        public decimal Price { get; set; }      // Selling price per unit at time of sale
        public decimal CostPrice { get; set; }  // Cost price snapshot at time of sale (for profit)

        public decimal Total     => Price     * Quantity; // Revenue
        public decimal TotalCost => CostPrice * Quantity; // Cost of goods sold
        public decimal Profit    => (Price - CostPrice) * Quantity;
    }
}