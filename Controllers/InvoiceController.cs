// Controllers/InvoiceController.cs
using InventoryManagement.Models;
using InventoryManagement.Services.Interfaces;
using InventoryManagement.Services;
using InventoryManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Controllers
{
    // All authenticated roles can access invoices.
    // (Admin, Manager, Staff — Staff can only create invoices.)
    [Authorize]
    public class InvoiceController : Controller
    {
        private readonly IInvoiceService _invoiceService;
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;
        private readonly IAreaService _areaService;
        private readonly ICustomerProductPriceService _customerProductService;
        private readonly IProfitService _profitService;
        private readonly ILogger<InvoiceController> _logger;

        public InvoiceController(
            IInvoiceService invoiceService,
            ICustomerService customerService,
            IProductService productService,
            ICustomerProductPriceService customerProductPriceService,
            IProfitService profitService,
            ILogger<InvoiceController> logger,
            IAreaService areaService)
        {
            _invoiceService        = invoiceService;
            _customerService       = customerService;
            _productService        = productService;
            _areaService           = areaService;
            _logger                = logger;
            _customerProductService= customerProductPriceService;
            _profitService         = profitService;
        }

        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Index()
        {
            var invoices = await _invoiceService.GetAllInvoicesAsync();
            return View(invoices);
        }

        // GET: Invoice/Create
        public async Task<IActionResult> Create()
        {
            var model = new InvoiceViewModel
            {
                Areas     = await _areaService.GetAllAreasAsync(),
                Customers = await _customerService.GetAllCustomerAsync(),
                Products  = await _productService.GetAllProductsAsync()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(InvoiceViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Areas     = await _areaService.GetAllAreasAsync();
                model.Customers = await _customerService.GetAllCustomerAsync();
                model.Products  = await _productService.GetAllProductsAsync();
                return View(model);
            }

            var validItems = model.Items.Where(i => i.ProductId != 0).ToList();
            if (!validItems.Any())
            {
                ModelState.AddModelError("Items", "At least one product must be selected.");
                model.Areas     = await _areaService.GetAllAreasAsync();
                model.Customers = await _customerService.GetAllCustomerAsync();
                model.Products  = await _productService.GetAllProductsAsync();
                return View(model);
            }

            var productDict = (await _productService.GetAllProductsAsync()).ToDictionary(p => p.Id);
            var invoiceItems = new List<InvoiceItem>();

            foreach (var item in validItems)
            {
                if (!productDict.TryGetValue(item.ProductId, out var product))
                {
                    ModelState.AddModelError("Items", "Invalid product selected.");
                    model.Areas     = await _areaService.GetAllAreasAsync();
                    model.Customers = await _customerService.GetAllCustomerAsync();
                    model.Products  = await _productService.GetAllProductsAsync();
                    return View(model);
                }

                invoiceItems.Add(new InvoiceItem
                {
                    ProductId = product.Id,
                    Quantity  = item.Quantity,
                    Price     = item.Price,
                    CostPrice = product.CostPrice   // snapshot cost at time of sale
                });
            }

            var invoice = new Invoice
            {
                CustomerId  = model.CustomerId,
                TotalAmount = invoiceItems.Sum(i => i.Quantity * i.Price),
                Items       = invoiceItems
            };

            // 1. Save invoice
            await _invoiceService.CreateInvoiceAsync(invoice);

            // 2. Upsert customer-specific rates
            foreach (var item in validItems)
                await _customerProductService.UpsertRateAsync(model.CustomerId, item.ProductId, item.Price);

            // 3. Write profit records — reload with navigations for snapshots
            var savedInvoice = await _invoiceService.GetInvoiceByIdAsync(invoice.Id);
            if (savedInvoice != null)
                await _profitService.RecordProfitAsync(savedInvoice);

            return RedirectToAction(nameof(Index));
        }

        // GET: Invoice/Details/id
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Details(int id)
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
            if (invoice == null) return NotFound();
            return View(invoice);
        }

        // GET: Customer dropdown by area
        [HttpGet]
        public async Task<IActionResult> GetCustomersByArea(int areaId)
        {
            var customers = await _customerService.GetCustomersByAreaAsync(areaId);
            return Json(customers);
        }

        // GET: Invoice/Edit/id
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int id)
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
            if (invoice == null) return NotFound();

            var model = new InvoiceViewModel
            {
                Id         = invoice.Id,
                CustomerId = invoice.CustomerId,
                Customer   = invoice.Customer,
                TotalAmount= invoice.TotalAmount,
                Items      = invoice.Items.Select(item => new InvoiceItemViewModel
                {
                    ProductId   = item.ProductId,
                    ProductName = item.Product?.Name ?? "Unknown",
                    Price       = item.Price,
                    Quantity    = item.Quantity,
                    Total       = item.Total
                }).ToList(),
                Areas     = await _areaService.GetAllAreasAsync(),
                Customers = await _customerService.GetAllCustomerAsync(),
                Products  = await _productService.GetAllProductsAsync()
            };

            return View("Create", model);
        }

        [HttpGet]
        public async Task<JsonResult> GetProductRate(int customerId, int productId)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null) return Json(new { rate = 0m, isCustomRate = false });

            var customRate = await _customerProductService.GetCustomRateOrNullAsync(customerId, productId);
            if (customRate.HasValue)
                return Json(new { rate = customRate.Value, isCustomRate = true });

            return Json(new { rate = product.Price, isCustomRate = false });
        }

        // POST: Invoice/Edit/5
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(InvoiceViewModel model)
        {
            if (!ModelState.IsValid)
            {
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors.Select(e => e.ErrorMessage).ToList();
                    if (errors.Any()) _logger.LogWarning("ModelError {Field}: {@Errors}", key, errors);
                }
                model.Areas     = await _areaService.GetAllAreasAsync();
                model.Customers = await _customerService.GetAllCustomerAsync();
                model.Products  = await _productService.GetAllProductsAsync();
                return View("Create", model);
            }

            var validItems = model.Items.Where(i => i.ProductId != 0).ToList();
            if (!validItems.Any())
            {
                ModelState.AddModelError("Items", "At least one product must be selected.");
                model.Areas     = await _areaService.GetAllAreasAsync();
                model.Customers = await _customerService.GetAllCustomerAsync();
                model.Products  = await _productService.GetAllProductsAsync();
                return View("Create", model);
            }

            try
            {
                var productDict = (await _productService.GetAllProductsAsync()).ToDictionary(p => p.Id);
                var invoiceItems = new List<InvoiceItem>();

                foreach (var item in validItems)
                {
                    if (!productDict.TryGetValue(item.ProductId, out var product))
                        throw new InvalidOperationException($"Product ID {item.ProductId} not found.");

                    invoiceItems.Add(new InvoiceItem
                    {
                        ProductId = product.Id,
                        Quantity  = item.Quantity,
                        Price     = item.Price,         // use form price (may be custom rate)
                        CostPrice = product.CostPrice   // re-snapshot cost at time of edit
                    });
                }

                var invoice = new Invoice
                {
                    Id          = model.Id,
                    CustomerId  = model.CustomerId,
                    TotalAmount = invoiceItems.Sum(i => i.Quantity * i.Price),
                    Items       = invoiceItems
                };

                await _invoiceService.UpdateInvoiceAsync(invoice);

                // Replace profit records for this invoice
                var savedInvoice = await _invoiceService.GetInvoiceByIdAsync(invoice.Id);
                if (savedInvoice != null)
                    await _profitService.ReplaceProfitAsync(savedInvoice);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Invoice ID: {Id}", model.Id);
                ModelState.AddModelError("", "An error occurred while saving. Please try again.");
                model.Areas     = await _areaService.GetAllAreasAsync();
                model.Customers = await _customerService.GetAllCustomerAsync();
                model.Products  = await _productService.GetAllProductsAsync();
                return View("Create", model);
            }
        }
    }
}
