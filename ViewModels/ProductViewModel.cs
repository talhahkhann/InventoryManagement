using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Product Name")]
        public string Name { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Selling price must be greater than 0.")]
        [Display(Name = "Selling Price")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        [Display(Name = "Cost Price")]
        public decimal CostPrice { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        [Display(Name = "Stock Alert Threshold")]
        public int StockThreshold { get; set; } = 10;

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
    }
}
