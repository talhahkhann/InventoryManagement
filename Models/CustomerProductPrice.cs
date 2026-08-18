// Models/CustomerProductPrice.cs
namespace InventoryManagement.Models
{
    public class CustomerProductPrice
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public decimal Rate { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}