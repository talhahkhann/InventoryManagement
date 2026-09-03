using System.ComponentModel.DataAnnotations;
using InventoryManagement.Models;

namespace InventoryManagement.ViewModels
{
    // ─────────────────────────────────────────────────────────────────────────
    // Line-item row on the Create / Details form
    // ─────────────────────────────────────────────────────────────────────────
    public class PurchaseOrderItemViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select a product.")]
        [Display(Name = "Product")]
        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        [Required]
        [Range(1, 100000, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; } = 1;

        [Required]
        [Range(0.01, 1_000_000, ErrorMessage = "Unit cost must be greater than 0.")]
        [Display(Name = "Unit Cost (PKR)")]
        public decimal UnitCost { get; set; }

        public decimal LineTotal => UnitCost * Quantity;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Create / Edit form
    // ─────────────────────────────────────────────────────────────────────────
    public class PurchaseOrderCreateViewModel
    {
        [Required(ErrorMessage = "Please select a supplier.")]
        [Display(Name = "Supplier")]
        public int SupplierId { get; set; }

        [Required]
        [Display(Name = "Order Date")]
        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; } = DateTime.Today;

        [MaxLength(500)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        public List<PurchaseOrderItemViewModel> Items { get; set; } = new()
        {
            new PurchaseOrderItemViewModel()
        };

        // Populated by controller for dropdowns
        public List<Supplier> Suppliers { get; set; } = new();
        public List<Product>  Products  { get; set; } = new();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // List / Details view
    // ─────────────────────────────────────────────────────────────────────────
    public class PurchaseOrderListViewModel
    {
        public int                  Id           { get; set; }
        public string               SupplierName { get; set; } = string.Empty;
        public DateTime             OrderDate    { get; set; }
        public DateTime?            ReceivedDate { get; set; }
        public PurchaseOrderStatus  Status       { get; set; }
        public decimal              TotalCost    { get; set; }
        public int                  ItemCount    { get; set; }
        public string?              Notes        { get; set; }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Details page (single PO with all items)
    // ─────────────────────────────────────────────────────────────────────────
    public class PurchaseOrderDetailsViewModel
    {
        public int                         Id           { get; set; }
        public string                      SupplierName { get; set; } = string.Empty;
        public string?                     SupplierPhone { get; set; }
        public DateTime                    OrderDate    { get; set; }
        public DateTime?                   ReceivedDate { get; set; }
        public PurchaseOrderStatus         Status       { get; set; }
        public decimal                     TotalCost    { get; set; }
        public string?                     Notes        { get; set; }
        public string?                     ReceivedBy   { get; set; }
        public List<PurchaseOrderItemViewModel> Items   { get; set; } = new();
    }
}
