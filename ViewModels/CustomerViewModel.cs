using System.ComponentModel.DataAnnotations;
using InventoryManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.ViewModels
{
    public class CustomerViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Customer Name")]
        public required string Name { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [Phone]
        public required string PhoneNumber { get; set; }
        public int AreaId { get; set; }

        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
}
