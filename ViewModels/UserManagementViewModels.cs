using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.ViewModels
{
    /// <summary>Row in the user list table.</summary>
    public class UserListViewModel
    {
        public string Id           { get; set; } = string.Empty;
        public string FullName     { get; set; } = string.Empty;
        public string Email        { get; set; } = string.Empty;
        public string PhoneNumber  { get; set; } = string.Empty;
        public string Role         { get; set; } = "No Role";
        public string Country      { get; set; } = string.Empty;
    }

    /// <summary>Form for Admin to create a new user and assign a role.</summary>
    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "Full name is required")]
        [MaxLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [MinLength(4, ErrorMessage = "Password must be at least 4 characters")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; } = string.Empty;

        [Phone]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [MaxLength(250)]
        public string? Address { get; set; }

        [MaxLength(50)]
        public string? Country { get; set; }
    }

    /// <summary>Form for Admin to edit a user's details and role.</summary>
    public class EditUserViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "Full name is required")]
        [MaxLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [MaxLength(250)]
        public string? Address { get; set; }

        [MaxLength(50)]
        public string? Country { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; } = string.Empty;

        /// <summary>Leave blank to keep existing password.</summary>
        [DataType(DataType.Password)]
        [Display(Name = "New Password (leave blank to keep current)")]
        public string? NewPassword { get; set; }
    }
}
