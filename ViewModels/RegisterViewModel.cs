using System;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Full name is required")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [MinLength(4, ErrorMessage = "Password must be at least 4 characters")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; }

        public string Address { get; set; }
        public string PhoneNumber { get; set; }

        public string? City { get; set; }
        public string Country { get; set; }

        // [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
    }
}
