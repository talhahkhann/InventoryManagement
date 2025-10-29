using InventoryManagement.Models;

namespace InventoryManagement.ViewModels
{
    public class InvoiceViewModel
    {
        // Essential for Edit mode
        public int Id { get; set; }

        public int CustomerId { get; set; }
        
        // Optional: Include Customer for display (not required for binding)
        public Customer? Customer { get; set; }

        public List<InvoiceItemViewModel> Items { get; set; } = new();
        
        public decimal TotalAmount { get; set; }

        // For dropdowns
        public IEnumerable<Area> Areas { get; set; } = new List<Area>();
        public IEnumerable<Customer> Customers { get; set; } = new List<Customer>();
        public IEnumerable<Product> Products { get; set; } = new List<Product>();
    }
}