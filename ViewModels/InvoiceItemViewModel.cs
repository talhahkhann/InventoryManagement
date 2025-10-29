using InventoryManagement.Models;

namespace InventoryManagement.ViewModels
{
    public class InvoiceItemViewModel
    {
        public int ProductId { get; set; }
        
        // Required for autocomplete display and form binding
        public string ProductName { get; set; } = string.Empty;
        
        // Required to show and calculate price
        public decimal Price { get; set; }
        
        public int Quantity { get; set; } = 1;
        
        // Required for total column and grand total calculation
        public decimal Total { get; set; }
    }
}