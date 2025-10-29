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
        public decimal Price { get; set; }

        // Foreign Key to Category
        [Required]
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
