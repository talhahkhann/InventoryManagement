using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.ViewModels
{
    public class SupplierViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Supplier name is required.")]
        [MaxLength(150)]
        [Display(Name = "Supplier Name")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(150)]
        [Display(Name = "Contact Person")]
        public string? ContactPerson { get; set; }

        [Phone(ErrorMessage = "Invalid phone number.")]
        [MaxLength(20)]
        [Display(Name = "Phone")]
        public string? Phone { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address.")]
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

        // For list view — how many POs
        public int PurchaseOrderCount { get; set; }
    }
}
