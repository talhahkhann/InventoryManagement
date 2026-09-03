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

        /// <summary>
        /// Returns the customer with Area, and all Invoices → Items → Product
        /// eagerly loaded. Returns null if customer not found.
        /// </summary>
        Task<Customer?> GetWithHistoryAsync(int customerId);
    }
}