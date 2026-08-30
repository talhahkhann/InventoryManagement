using InventoryManagement.Services.Interfaces;
using InventoryManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Controllers
{
    // Profit analytics — Admin and Manager only.
    [Authorize(Roles = "Admin,Manager")]
    public class ProfitController : Controller
    {
        private readonly IProfitService _profitService;
        private readonly ILogger<ProfitController> _logger;

        public ProfitController(IProfitService profitService, ILogger<ProfitController> logger)
        {
            _profitService = profitService;
            _logger        = logger;
        }

        // GET /Profit
        public async Task<IActionResult> Index(int? year, int? month)
        {
            var filter = new ProfitFilterViewModel { Year = year ?? DateTime.Today.Year, Month = month };
            var vm     = await _profitService.GetDashboardAsync(filter);
            return View(vm);
        }

        // GET /Profit/ByMonth?year=2026
        public async Task<IActionResult> ByMonth(int? year)
        {
            var years      = await _profitService.GetAvailableYearsAsync();
            int selYear    = year ?? (years.Any() ? years.First() : DateTime.Today.Year);
            var data       = await _profitService.GetByMonthAsync(selYear);
            ViewBag.SelectedYear   = selYear;
            ViewBag.AvailableYears = years;
            return View(data);
        }

        // GET /Profit/ByYear
        public async Task<IActionResult> ByYear()
        {
            var data = await _profitService.GetByYearAsync();
            return View(data);
        }

        // GET /Profit/ByProduct?year=2026&month=1
        public async Task<IActionResult> ByProduct(int? year, int? month)
        {
            var years  = await _profitService.GetAvailableYearsAsync();
            var filter = new ProfitFilterViewModel
            {
                Year  = year  ?? (years.Any() ? years.First() : DateTime.Today.Year),
                Month = month
            };
            var data = await _profitService.GetByProductAsync(filter);
            ViewBag.Filter         = filter;
            ViewBag.AvailableYears = years;
            return View(data);
        }

        // GET /Profit/ByCustomer?year=2026&month=1
        public async Task<IActionResult> ByCustomer(int? year, int? month)
        {
            var years  = await _profitService.GetAvailableYearsAsync();
            var filter = new ProfitFilterViewModel
            {
                Year  = year  ?? (years.Any() ? years.First() : DateTime.Today.Year),
                Month = month
            };
            var data = await _profitService.GetByCustomerAsync(filter);
            ViewBag.Filter         = filter;
            ViewBag.AvailableYears = years;
            return View(data);
        }

        // GET /Profit/ByCategory?year=2026
        public async Task<IActionResult> ByCategory(int? year)
        {
            var years  = await _profitService.GetAvailableYearsAsync();
            var filter = new ProfitFilterViewModel
            {
                Year = year ?? (years.Any() ? years.First() : DateTime.Today.Year)
            };
            var data = await _profitService.GetByCategoryAsync(filter);
            ViewBag.Filter         = filter;
            ViewBag.AvailableYears = years;
            return View(data);
        }

        // GET /Profit/GetChartData?year=2026
        [HttpGet]
        public async Task<IActionResult> GetChartData(int? year)
        {
            int y      = year ?? DateTime.Today.Year;
            var monthly = await _profitService.GetByMonthAsync(y);
            return Json(new
            {
                labels  = monthly.Select(m => m.MonthName),
                revenue = monthly.Select(m => m.Revenue),
                cost    = monthly.Select(m => m.Cost),
                profit  = monthly.Select(m => m.Profit),
                margin  = monthly.Select(m => m.MarginPct)
            });
        }

        // GET /Profit/ExportCsv?view=ByProduct&year=2026
        [HttpGet]
        public async Task<IActionResult> ExportCsv(string view = "ByMonth", int? year = null, int? month = null)
        {
            var filter = new ProfitFilterViewModel { Year = year ?? DateTime.Today.Year, Month = month };
            string csv; string filename;

            switch (view.ToLowerInvariant())
            {
                case "byyear":
                    var y = await _profitService.GetByYearAsync();
                    csv = CsvYear(y); filename = "profit_by_year.csv"; break;
                case "byproduct":
                    var p = await _profitService.GetByProductAsync(filter);
                    csv = CsvProduct(p); filename = $"profit_by_product_{filter.Year}.csv"; break;
                case "bycustomer":
                    var c = await _profitService.GetByCustomerAsync(filter);
                    csv = CsvCustomer(c); filename = $"profit_by_customer_{filter.Year}.csv"; break;
                default:
                    var m = await _profitService.GetByMonthAsync(filter.Year!.Value);
                    csv = CsvMonth(m); filename = $"profit_by_month_{filter.Year}.csv"; break;
            }

            return File(System.Text.Encoding.UTF8.GetBytes(csv), "text/csv", filename);
        }

        private static string CsvMonth(List<ProfitByMonthViewModel> d)
        {
            var sb = new System.Text.StringBuilder("Month,Revenue,Cost,Profit,Margin %,Invoices,Units\n");
            foreach (var r in d) sb.AppendLine($"{r.MonthName},{r.Revenue:F2},{r.Cost:F2},{r.Profit:F2},{r.MarginPct:F2},{r.InvoiceCount},{r.UnitsSold}");
            return sb.ToString();
        }
        private static string CsvYear(List<ProfitByYearViewModel> d)
        {
            var sb = new System.Text.StringBuilder("Year,Revenue,Cost,Profit,Margin %,Invoices,Units\n");
            foreach (var r in d) sb.AppendLine($"{r.Year},{r.Revenue:F2},{r.Cost:F2},{r.Profit:F2},{r.MarginPct:F2},{r.InvoiceCount},{r.UnitsSold}");
            return sb.ToString();
        }
        private static string CsvProduct(List<ProfitByProductViewModel> d)
        {
            var sb = new System.Text.StringBuilder("Product,Category,Revenue,Cost,Profit,Margin %,Units,Avg Sell,Avg Cost\n");
            foreach (var r in d) sb.AppendLine($"\"{r.ProductName}\",\"{r.CategoryName}\",{r.Revenue:F2},{r.Cost:F2},{r.Profit:F2},{r.MarginPct:F2},{r.UnitsSold},{r.AvgSellingPrice:F2},{r.AvgCostPrice:F2}");
            return sb.ToString();
        }
        private static string CsvCustomer(List<ProfitByCustomerViewModel> d)
        {
            var sb = new System.Text.StringBuilder("Customer,Area,Revenue,Cost,Profit,Margin %,Invoices,Units\n");
            foreach (var r in d) sb.AppendLine($"\"{r.CustomerName}\",\"{r.AreaName}\",{r.Revenue:F2},{r.Cost:F2},{r.Profit:F2},{r.MarginPct:F2},{r.InvoiceCount},{r.UnitsSold}");
            return sb.ToString();
        }
    }
}
