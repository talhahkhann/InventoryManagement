using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Models
{
    public class Invoice
    {
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; } // FK
        public Customer Customer { get; set; }

        public DateTime InvoiceDate { get; set; } = DateTime.Now;

        public decimal TotalAmount { get; set; }

        public List<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
    }
}
