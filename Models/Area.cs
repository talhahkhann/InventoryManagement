namespace InventoryManagement.Models
{
    public class Area
    {
        public int AreaId { get; set; }   // Primary Key
        public string AreaName { get; set; }

        // Navigation Property - One Area has many Customers
        public ICollection<Customer> Customers { get; set; }
    }
}
