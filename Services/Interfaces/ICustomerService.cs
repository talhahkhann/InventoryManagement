using InventoryManagement.Models;
using InventoryManagement.ViewModels;

namespace InventoryManagement.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>> GetAllCustomerAsync();
        Task<Customer> GetCustomerByIdAsync(int id);
        Task AddCustomerAsync(Customer customer);
        Task UpdateCustomerAsync(Customer customer);
        Task DeleteCustomerAsync(int id);
        Task<IEnumerable<Customer>> GetCustomersByAreaAsync(int areaId);

        /// <summary>
        /// Returns a fully populated CustomerHistoryViewModel for the given
        /// customer, including all invoices and computed stats.
        /// Returns null if the customer does not exist.
        /// </summary>
        Task<CustomerHistoryViewModel?> GetHistoryAsync(int customerId);
    }
}