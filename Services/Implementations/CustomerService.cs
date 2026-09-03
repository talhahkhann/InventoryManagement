using InventoryManagement.Models;
using InventoryManagement.Repositories.Interfaces;
using InventoryManagement.Services.Interfaces;
using InventoryManagement.ViewModels;

namespace InventoryManagement.Services.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<IEnumerable<Customer>> GetAllCustomerAsync()
            => await _customerRepository.GetAllCustomerAsync();

        public async Task<Customer> GetCustomerByIdAsync(int id)
            => await _customerRepository.GetCustomerByIdAsync(id);

        public async Task AddCustomerAsync(Customer customer)
            => await _customerRepository.AddCustomerAsync(customer);

        public async Task UpdateCustomerAsync(Customer customer)
            => await _customerRepository.UpdateCustomerAsync(customer);

        public async Task DeleteCustomerAsync(int id)
            => await _customerRepository.DeleteCustomerAsync(id);

        public async Task<IEnumerable<Customer>> GetCustomersByAreaAsync(int areaId)
            => await _customerRepository.GetCustomersByAreaAsync(areaId);

        // ── Purchase History ─────────────────────────────────────────────
        public async Task<CustomerHistoryViewModel?> GetHistoryAsync(int customerId)
        {
            var customer = await _customerRepository.GetWithHistoryAsync(customerId);
            if (customer == null) return null;

            var invoices = customer.Invoices
                .OrderByDescending(i => i.InvoiceDate)
                .ToList();

            // ── Per-invoice rows ─────────────────────────────────────────
            var rows = invoices.Select(inv =>
            {
                // Top product on this invoice by quantity
                var topItem = inv.Items
                    .OrderByDescending(it => it.Quantity)
                    .FirstOrDefault();

                return new CustomerInvoiceRowViewModel
                {
                    Id         = inv.Id,
                    Date       = inv.InvoiceDate,
                    ItemCount  = inv.Items.Count,
                    Total      = inv.TotalAmount,
                    TopProduct = topItem?.Product?.Name ?? "—"
                };
            }).ToList();

            // ── Aggregate stats ──────────────────────────────────────────
            decimal totalSpend = invoices.Sum(i => i.TotalAmount);
            int     count      = invoices.Count;

            // Top product across all invoices by total units sold
            var topProductGroup = invoices
                .SelectMany(i => i.Items)
                .GroupBy(it => it.Product?.Name ?? "Unknown")
                .Select(g => new { Name = g.Key, Units = g.Sum(x => x.Quantity) })
                .OrderByDescending(g => g.Units)
                .FirstOrDefault();

            return new CustomerHistoryViewModel
            {
                Id             = customer.Id,
                Name           = customer.Name        ?? string.Empty,
                Email          = customer.Email       ?? string.Empty,
                Phone          = customer.PhoneNumber ?? string.Empty,
                Address        = customer.Address     ?? string.Empty,
                City           = customer.City        ?? string.Empty,
                Country        = customer.Country     ?? string.Empty,
                AreaName       = customer.Area?.AreaName ?? "—",

                TotalInvoices  = count,
                TotalSpend     = totalSpend,
                AvgOrderValue  = count > 0 ? totalSpend / count : 0m,
                FirstPurchase  = invoices.Any() ? invoices.Min(i => i.InvoiceDate)  : null,
                LastPurchase   = invoices.Any() ? invoices.Max(i => i.InvoiceDate)  : null,

                TopProduct     = topProductGroup?.Name  ?? "—",
                TopProductUnits= topProductGroup?.Units ?? 0,

                Invoices       = rows
            };
        }
    }
}
