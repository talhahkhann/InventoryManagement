using InventoryManagement.Models;
using InventoryManagement.Repositories.Interfaces;
using InventoryManagement.Services.Interfaces;
using InventoryManagement.ViewModels;

namespace InventoryManagement.Services.Implementations
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly IPurchaseOrderRepository _poRepo;
        private readonly IProductService          _productService;
        private readonly IStockAlertService       _alertService;
        private readonly ILogger<PurchaseOrderService> _logger;

        public PurchaseOrderService(
            IPurchaseOrderRepository    poRepo,
            IProductService             productService,
            IStockAlertService          alertService,
            ILogger<PurchaseOrderService> logger)
        {
            _poRepo         = poRepo;
            _productService = productService;
            _alertService   = alertService;
            _logger         = logger;
        }

        public Task<IEnumerable<PurchaseOrder>> GetAllAsync()    => _poRepo.GetAllAsync();
        public Task<PurchaseOrder?>             GetByIdAsync(int id) => _poRepo.GetByIdAsync(id);

        // ── Create Draft ─────────────────────────────────────────────
        public async Task<PurchaseOrder> CreateAsync(
            PurchaseOrderCreateViewModel vm, string createdByEmail)
        {
            var po = new PurchaseOrder
            {
                SupplierId = vm.SupplierId,
                OrderDate  = vm.OrderDate.ToUniversalTime(),
                Notes      = vm.Notes,
                Status     = PurchaseOrderStatus.Draft,
                CreatedAt  = DateTime.UtcNow,
                Items      = vm.Items
                    .Where(i => i.ProductId != 0 && i.Quantity > 0)
                    .Select(i => new PurchaseOrderItem
                    {
                        ProductId = i.ProductId,
                        Quantity  = i.Quantity,
                        UnitCost  = i.UnitCost
                    }).ToList()
            };

            po.TotalCost = po.Items.Sum(i => i.UnitCost * i.Quantity);
            return await _poRepo.CreateAsync(po);
        }

        // ── Receive Order ────────────────────────────────────────────
        /// <summary>
        /// Atomically receives a Draft PO:
        ///   1. Load PO with items + products
        ///   2. For each item: add stock, update product CostPrice, check alerts
        ///   3. Mark PO as Received
        /// Throws if already Received or Cancelled.
        /// </summary>
        public async Task ReceiveOrderAsync(int purchaseOrderId, string receivedByEmail)
        {
            var po = await _poRepo.GetByIdAsync(purchaseOrderId)
                     ?? throw new InvalidOperationException($"Purchase Order #{purchaseOrderId} not found.");

            if (po.Status == PurchaseOrderStatus.Received)
                throw new InvalidOperationException($"Purchase Order #{purchaseOrderId} has already been received.");

            if (po.Status == PurchaseOrderStatus.Cancelled)
                throw new InvalidOperationException($"Purchase Order #{purchaseOrderId} is cancelled and cannot be received.");

            foreach (var item in po.Items)
            {
                // 1. Add stock back (restore = stock increase)
                var product = await _productService.RestoreStockAsync(item.ProductId, item.Quantity);

                // 2. Update cost price to latest purchase cost
                product.CostPrice = item.UnitCost;
                await _productService.UpdateProductAsync(product);

                // 3. Auto-resolve any existing alert if stock is now above threshold
                //    or re-fire if still below (edge case: threshold > just-received qty)
                if (product.Quantity >= product.StockThreshold)
                    await _alertService.AutoResolveAlertsAsync(product.Id, product.Quantity);
                else
                    await _alertService.CreateOrUpdateAlertAsync(
                        product.Id, product.Quantity, product.StockThreshold);

                _logger.LogInformation(
                    "PO #{PoId} received: +{Qty} of '{Product}' — new stock {Stock}",
                    purchaseOrderId, item.Quantity, product.Name, product.Quantity);
            }

            // 4. Mark PO as received
            po.Status       = PurchaseOrderStatus.Received;
            po.ReceivedDate = DateTime.UtcNow;
            po.ReceivedBy   = receivedByEmail;
            await _poRepo.UpdateAsync(po);
        }

        // ── Delete Draft ─────────────────────────────────────────────
        public async Task DeleteAsync(int id)
        {
            var po = await _poRepo.GetByIdAsync(id)
                     ?? throw new InvalidOperationException($"Purchase Order #{id} not found.");

            if (po.Status == PurchaseOrderStatus.Received)
                throw new InvalidOperationException(
                    "Received orders cannot be deleted. They are part of inventory history.");

            await _poRepo.DeleteAsync(id);
        }
    }
}
