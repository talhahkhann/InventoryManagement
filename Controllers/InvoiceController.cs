// Controllers/InvoiceController.cs
using InventoryManagement.Models;
using InventoryManagement.Services.Interfaces;
using InventoryManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly IInvoiceService _invoiceService;
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;
        private readonly IAreaService _areaService;
        private readonly ICustomerProductPriceService _customerProductService;
        private readonly ILogger<InvoiceController> _logger;

        public InvoiceController(
            IInvoiceService invoiceService,
            ICustomerService customerService,
            IProductService productService,
            ICustomerProductPriceService customerProductPriceService,
            ILogger<InvoiceController> logger,
            IAreaService areaService)
        {
            _invoiceService = invoiceService;
            _customerService = customerService;
            _productService = productService;
            _areaService = areaService;
            _logger = logger;
            _customerProductService = customerProductPriceService;
        }

        public async Task<IActionResult> Index()
        {
            var invoices = await _invoiceService.GetAllInvoicesAsync();
            return View(invoices);
        }
        // GET : Invoice/Create
        public async Task<IActionResult> Create()
        {
            var model = new InvoiceViewModel
            {
                Areas = await _areaService.GetAllAreasAsync(),
                Customers = await _customerService.GetAllCustomerAsync(),
                Products = await _productService.GetAllProductsAsync()
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Create(InvoiceViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Areas = await _areaService.GetAllAreasAsync();
                model.Customers = await _customerService.GetAllCustomerAsync();
                model.Products = await _productService.GetAllProductsAsync();
                return View(model);
            }

            // Remove empty rows
            var validItems = model.Items
                .Where(i => i.ProductId != 0)
                .ToList();

            if (!validItems.Any())
            {
                ModelState.AddModelError(
                    "Items",
                    "At least one product must be selected."
                );

                model.Areas = await _areaService.GetAllAreasAsync();
                model.Customers = await _customerService.GetAllCustomerAsync();
                model.Products = await _productService.GetAllProductsAsync();

                return View(model);
            }

            // Get products
            var productDict = (await _productService.GetAllProductsAsync())
                .ToDictionary(p => p.Id, p => p);

            var invoiceItems = new List<InvoiceItem>();

            foreach (var item in validItems)
            {
                if (!productDict.TryGetValue(item.ProductId, out var product))
                {
                    ModelState.AddModelError(
                        "Items",
                        "Invalid product selected."
                    );

                    model.Areas = await _areaService.GetAllAreasAsync();
                    model.Customers = await _customerService.GetAllCustomerAsync();
                    model.Products = await _productService.GetAllProductsAsync();

                    return View(model);
                }

                invoiceItems.Add(new InvoiceItem
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,

                    // Use the price/rate selected on the invoice
                    Price = item.Price
                });
            }

            // Calculate invoice total
            var totalAmount = invoiceItems.Sum(
                i => i.Quantity * i.Price
            );

            // Create invoice
            var invoice = new Invoice
            {
                CustomerId = model.CustomerId,
                TotalAmount = totalAmount,
                Items = invoiceItems
            };

            // 1. Save invoice first
            await _invoiceService.CreateInvoiceAsync(invoice);

            // 2. After invoice is successfully saved,
            //    save/update customer-specific rates
            foreach (var item in validItems)
            {
                await _customerProductService.UpsertRateAsync(
                    model.CustomerId,
                    item.ProductId,
                    item.Price
                );
            }

            // 3. Redirect after everything is done
            return RedirectToAction(nameof(Index));
        }     //GET: Invoice/Details/id
        public async Task<IActionResult> Details(int id)
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
            if (invoice == null) return NotFound();
            return View(invoice);
        }
        //GET: Customer/DropDown Menu
        [HttpGet]
        public async Task<IActionResult> GetCustomersByArea(int areaId)
        {
            var customers = await _customerService.GetCustomersByAreaAsync(areaId);
            return Json(customers);
        }
        //GET: Invoice/Edit/id
        public async Task<IActionResult> Edit(int id)
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
            if (invoice == null)
                return NotFound();


            // Map Invoice to InvoiceViewModel
            var model = new InvoiceViewModel
            {
                Id = invoice.Id,
                CustomerId = invoice.CustomerId,
                Customer = invoice.Customer, // Ensure your Invoice model includes Customer
                TotalAmount = invoice.TotalAmount,
                Items = invoice.Items.Select(item => new InvoiceItemViewModel
                {
                    ProductId = item.ProductId,
                    ProductName = item.Product?.Name ?? "Unknown Product", // Ensure Product is loaded
                    Price = item.Price,
                    Quantity = item.Quantity,
                    Total = item.Total
                }).ToList(),



                // Load dropdown data
                Areas = await _areaService.GetAllAreasAsync(),
                Customers = await _customerService.GetAllCustomerAsync(),
                Products = await _productService.GetAllProductsAsync(),

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
        public async Task<IActionResult> Edit(InvoiceViewModel model)
        {

            if (!ModelState.IsValid)
            {

                // Log all model errors
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors.Select(e => e.ErrorMessage).ToList();
                    if (errors.Any())
                    {
                        _logger.LogWarning("ModelError - Field: {Field}, Errors: {@Errors}", key, errors);
                    }
                }

                // Reload dropdowns
                model.Areas = await _areaService.GetAllAreasAsync();
                model.Customers = await _customerService.GetAllCustomerAsync();
                model.Products = await _productService.GetAllProductsAsync();

                return View("Create", model);
            }

            var validItems = model.Items.Where(i => i.ProductId != 0).ToList();
            if (!validItems.Any())
            {
                _logger.LogWarning("No valid items provided for Invoice ID: {InvoiceId}", model.Id);
                ModelState.AddModelError("Items", "At least one product must be selected.");

                model.Areas = await _areaService.GetAllAreasAsync();
                model.Customers = await _customerService.GetAllCustomerAsync();
                model.Products = await _productService.GetAllProductsAsync();

                return View("Create", model);
            }

            try
            {
                var allProducts = await _productService.GetAllProductsAsync();
                var productDict = allProducts.ToDictionary(p => p.Id, p => p);

                var invoiceItems = new List<InvoiceItem>();
                foreach (var item in validItems)
                {
                    if (!productDict.TryGetValue(item.ProductId, out var product))
                    {
                        throw new InvalidOperationException($"Product ID {item.ProductId} not found.");
                    }

                    _logger.LogInformation("Adding item: Product={Product}, Qty={Qty}, Price={Price}",
                        product.Name, item.Quantity, product.Price);

                    invoiceItems.Add(new InvoiceItem
                    {
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        Price = product.Price
                    });
                }

                var totalAmount = invoiceItems.Sum(i => i.Quantity * i.Price);
                _logger.LogInformation("Computed TotalAmount: {TotalAmount} for Invoice ID: {InvoiceId}",
                    totalAmount, model.Id);

                var invoice = new Invoice
                {
                    Id = model.Id,
                    CustomerId = model.CustomerId,
                    TotalAmount = totalAmount,
                    Items = invoiceItems
                };

                await _invoiceService.UpdateInvoiceAsync(invoice);
                _logger.LogInformation("Invoice ID {InvoiceId} updated successfully.", model.Id);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Invoice ID: {InvoiceId}", model.Id);
                ModelState.AddModelError("", "An error occurred while saving the invoice. Please try again.");

                model.Areas = await _areaService.GetAllAreasAsync();
                model.Customers = await _customerService.GetAllCustomerAsync();
                model.Products = await _productService.GetAllProductsAsync();

                return View("Create", model);
            }
        }
    }
}
