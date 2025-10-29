using InventoryManagement.Models;

namespace InventoryManagement.Repositories.Interfaces
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetAllCustomerAsync();
        Task<Customer> GetCustomerByIdAsync(int id);
        Task AddCustomerAsync(Customer customer);
        Task UpdateCustomerAsync(Customer customer);
        Task DeleteCustomerAsync(int id);
         Task<IEnumerable<Customer>> GetCustomersByAreaAsync(int areaId);
    }
}