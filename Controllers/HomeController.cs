using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using InventoryManagement.Models;
using InventoryManagement.Services.Interfaces;


namespace InventoryManagement.Controllers;

public class HomeController : Controller
{
    private readonly ICustomerService _customerService;
    private readonly IInvoiceService _invoiceService;
    private readonly IProductService _productService;


    public HomeController(ICustomerService customerService, IProductService productService, IInvoiceService invoiceService)
    {
        _customerService = customerService;
        _invoiceService = invoiceService;
        _productService = productService;
    }
   public async Task<IActionResult> Index()
{
    var customers = await _customerService.GetAllCustomerAsync();
    var invoices = await _invoiceService.GetAllInvoicesAsync();
    var products = await _productService.GetAllProductsAsync();

    var model = new DashboardViewModel
    {
        CustomerCount = customers.Count(),
        InvoiceCount = invoices.Count(),
        ProductCount = products.Count()
    };

    return View(model);
}


    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
