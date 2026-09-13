using InventoryManagement.Data;
using InventoryManagement.Models;
using InventoryManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Repositories.Implementations
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _context;

        public CustomerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddCustomerAsync(Customer customer)
        {
            await _context.Set<Customer>().AddAsync(customer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCustomerAsync(int id)
        {
            var customer = await GetCustomerByIdAsync(id);
            if (customer != null)
            {
                _context.Set<Customer>().Remove(customer);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Customer>> GetAllCustomerAsync()
            => await _context.Set<Customer>().ToListAsync();

        public async Task<Customer> GetCustomerByIdAsync(int id)
            => await _context.Set<Customer>().FindAsync(id);

        public async Task UpdateCustomerAsync(Customer customer)
        {
            _context.Set<Customer>().Update(customer);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Customer>> GetCustomersByAreaAsync(int areaId)
            => await _context.Customers.Where(c => c.AreaId == areaId).ToListAsync();

        public async Task<Customer?> GetWithHistoryAsync(int customerId)
            => await _context.Customers
                .Include(c => c.Area)
                .Include(c => c.Invoices)
                    .ThenInclude(i => i.Items)
                        .ThenInclude(it => it.Product)
                .FirstOrDefaultAsync(c => c.Id == customerId);
    }
}