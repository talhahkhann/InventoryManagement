using System;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(150)]
        public string Email { get; set; }

        [StringLength(15)]
        public string PhoneNumber { get; set; }

        [StringLength(200)]
        public string Address { get; set; }

        [StringLength(100)]
        public string City { get; set; }

        [StringLength(100)]
        public string Country { get; set; }
        //Foreign Key
        public int AreaId { get; set; }
        // Navigation Properties
        public Area Area { get; set; }
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

        public DateTime? DateOfBirth { get; set; }
    }
}
