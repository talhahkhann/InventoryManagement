using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Models
{
    public class Supplier
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        [Display(Name = "Supplier Name")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(150)]
        [Display(Name = "Contact Person")]
        public string? ContactPerson { get; set; }

        [Phone]
        [MaxLength(20)]
        [Display(Name = "Phone")]
        public string? Phone { get; set; }

        [EmailAddress]
        [MaxLength(150)]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [MaxLength(300)]
        [Display(Name = "Address")]
        public string? Address { get; set; }

        [MaxLength(100)]
        [Display(Name = "City")]
        public string? City { get; set; }

        [MaxLength(100)]
        [Display(Name = "Country")]
        public string? Country { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
    }
}
