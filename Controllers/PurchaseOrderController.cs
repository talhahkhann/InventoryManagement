using InventoryManagement.Models;
using InventoryManagement.Services.Interfaces;
using InventoryManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class PurchaseOrderController : Controller
    {
        private readonly IPurchaseOrderService _poService;
        private readonly ISupplierService      _supplierService;
        private readonly IProductService       _productService;
        private readonly ILogger<PurchaseOrderController> _logger;

        public PurchaseOrderController(
            IPurchaseOrderService           poService,
            ISupplierService                supplierService,
            IProductService                 productService,
            ILogger<PurchaseOrderController> logger)
        {
            _poService       = poService;
            _supplierService = supplierService;
            _productService  = productService;
            _logger          = logger;
        }

        // ── Index ────────────────────────────────────────────────────
        public async Task<IActionResult> Index()
        {
            var orders = await _poService.GetAllAsync();
            var vms = orders.Select(po => new PurchaseOrderListViewModel
            {
                Id           = po.Id,
                SupplierName = po.Supplier?.Name ?? "—",
                OrderDate    = po.OrderDate,
                ReceivedDate = po.ReceivedDate,
                Status       = po.Status,
                TotalCost    = po.TotalCost,
                ItemCount    = po.Items?.Count ?? 0,
                Notes        = po.Notes
            }).ToList();

            if (TempData["Success"] is string ok)  ViewBag.Success = ok;
            if (TempData["Error"]   is string err) ViewBag.Error   = err;
            return View(vms);
        }

        // ── Details ──────────────────────────────────────────────────
        public async Task<IActionResult> Details(int id)
        {
            var po = await _poService.GetByIdAsync(id);
            if (po == null) return NotFound();

            var vm = new PurchaseOrderDetailsViewModel
            {
                Id            = po.Id,
                SupplierName  = po.Supplier?.Name ?? "—",
                SupplierPhone = po.Supplier?.Phone,
                OrderDate     = po.OrderDate,
                ReceivedDate  = po.ReceivedDate,
                Status        = po.Status,
                TotalCost     = po.TotalCost,
                Notes         = po.Notes,
                ReceivedBy    = po.ReceivedBy,
                Items         = po.Items.Select(i => new PurchaseOrderItemViewModel
                {
                    Id          = i.Id,
                    ProductId   = i.ProductId,
                    ProductName = i.Product?.Name ?? "Unknown",
                    Quantity    = i.Quantity,
                    UnitCost    = i.UnitCost
                }).ToList()
            };

            return View(vm);
        }

        // ── Create GET ───────────────────────────────────────────────
        public async Task<IActionResult> Create()
        {
            var vm = new PurchaseOrderCreateViewModel
            {
                OrderDate = DateTime.Today,
                Suppliers = (await _supplierService.GetActiveAsync()).ToList(),
                Products  = (await _productService.GetAllProductsAsync()).ToList()
            };
            return View(vm);
        }

        // ── Create POST ──────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PurchaseOrderCreateViewModel vm)
        {
            // Filter out blank rows before validation
            vm.Items = vm.Items.Where(i => i.ProductId != 0).ToList();

            if (!vm.Items.Any())
                ModelState.AddModelError("Items", "Add at least one product.");

            if (!ModelState.IsValid)
            {
                vm.Suppliers = (await _supplierService.GetActiveAsync()).ToList();
                vm.Products  = (await _productService.GetAllProductsAsync()).ToList();
                return View(vm);
            }

            var po = await _poService.CreateAsync(vm, User.Identity?.Name ?? "system");
            TempData["Success"] = $"Purchase Order #{po.Id} created as Draft.";
            return RedirectToAction(nameof(Details), new { id = po.Id });
        }

        // ── Receive POST — the critical action ───────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Receive(int id)
        {
            try
            {
                await _poService.ReceiveOrderAsync(id, User.Identity?.Name ?? "system");
                TempData["Success"] = $"Purchase Order #{id} received. Stock has been updated.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error receiving PO #{Id}", id);
                TempData["Error"] = "An unexpected error occurred. Please try again.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // ── Delete POST — Draft only ─────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _poService.DeleteAsync(id);
                TempData["Success"] = $"Purchase Order #{id} deleted.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        // ── JSON helper for product autocomplete ─────────────────────
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Json(products.Select(p => new
            {
                id       = p.Id,
                name     = p.Name,
                costPrice = p.CostPrice,
                quantity = p.Quantity
            }));
        }
    }
}
