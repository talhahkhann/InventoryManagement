using InventoryManagement.Models;
using InventoryManagement.Repositories.Interfaces;
using InventoryManagement.Services.Interfaces;
using InventoryManagement.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace InventoryManagement.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRespository _accountRepository;
        public AccountService(IAccountRespository accountRespository)
        {
            _accountRepository = accountRespository;
        }
        public async Task<IdentityResult> RegisterAsync(RegisterViewModel model)
        {
            var user = new ApplicationUser
            {
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                FullName = model.FullName,
                UserName = model.Email,
                Address = model.Address,
                Country = model.Country
            };
            var result = await _accountRepository.CreateUserAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _accountRepository.AddToRoleAsync(user, model.Role);
            }
            return result;
        }
        public async Task<ApplicationUser> LoginAsync(LoginViewModel model)
        {
            return await _accountRepository.GetByEmailAsync(model.Email);
        }
    }
}