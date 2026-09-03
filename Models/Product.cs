using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; }

        [Required]
        [Range(0, 100000)]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, 100000)]
        public decimal Price { get; set; }          // Selling / list price

        [Range(0, 100000)]
        public decimal CostPrice { get; set; }      // Purchase / cost price (for profit calculation)

        /// <summary>
        /// When stock falls below this number, a StockAlert is automatically created.
        /// Default 10 — Admin/Manager can adjust per product.
        /// </summary>
        [Range(0, 100000)]
        [Display(Name = "Stock Alert Threshold")]
        public int StockThreshold { get; set; } = 10;

        // Foreign Key to Category
        [Required]
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
