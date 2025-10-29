using InventoryManagement.Models;
using Microsoft.AspNetCore.Identity;
namespace InventoryManagement.Repositories.Interfaces{
public interface IAccountRespository{

    Task<ApplicationUser> GetByEmailAsync(string email);
    Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);
    Task<bool> CheckRoleAsync(ApplicationUser user, string role);
    Task AddToRoleAsync(ApplicationUser user, string roles);
}
}