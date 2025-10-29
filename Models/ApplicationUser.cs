using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace InventoryManagement.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }
        [MaxLength(250)]
        public string Address { get; set; }
        [Phone]
        override
        public string PhoneNumber { get; set; }
        [EmailAddress]
        [Required]
        public override string Email { get; set; }
        [MaxLength(50)]
        public string Country { get; set; }
        //Optional extra fields
        public DateTime? DateoBirth { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}