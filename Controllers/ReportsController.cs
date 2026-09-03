using InventoryManagement.Services;
using InventoryManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace InventoryManagement.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class ReportsController : Controller
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        // ── Helpers ───────────────────────────────────────────────────────
        private static SalesReportFilterViewModel BuildFilter(DateTime? from, DateTime? to)
            => new() { From = from, To = to };

        // ── Sales Per Product ─────────────────────────────────────────────
        public async Task<IActionResult> SalesPerProduct(DateTime? from, DateTime? to)
        {
            var vm = await _reportService.GetSalesPerProductAsync(BuildFilter(from, to));
            return View(vm);
        }

        // ── Sales Per Category ────────────────────────────────────────────
        public async Task<IActionResult> SalesPerCategory(DateTime? from, DateTime? to)
        {
            var vm = await _reportService.GetSalesPerCategoryAsync(BuildFilter(from, to));
            return View(vm);
        }

        // ── Top Customers ─────────────────────────────────────────────────
        public async Task<IActionResult> TopCustomers(DateTime? from, DateTime? to, int top = 20)
        {
            var vm = await _reportService.GetTopCustomersAsync(BuildFilter(from, to), top);
            ViewBag.Top = top;
            return View(vm);
        }

        // ── Top Products ──────────────────────────────────────────────────
        public async Task<IActionResult> TopProducts(DateTime? from, DateTime? to, int top = 20)
        {
            var vm = await _reportService.GetTopProductsAsync(BuildFilter(from, to), top);
            ViewBag.Top = top;
            return View(vm);
        }

        // ── CSV Export ────────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> ExportCsv(
            string report = "SalesPerProduct",
            DateTime? from = null,
            DateTime? to   = null,
            int top = 20)
        {
            var filter   = BuildFilter(from, to);
            string csv;
            string filename;

            switch (report.ToLowerInvariant())
            {
                case "salespercategory":
                    var catVm  = await _reportService.GetSalesPerCategoryAsync(filter);
                    csv        = CsvCategory(catVm.Data, filter);
                    filename   = $"sales_per_category_{CsvDateRange(filter)}.csv";
                    break;

                case "topcustomers":
                    var custVm = await _reportService.GetTopCustomersAsync(filter, top);
                    csv        = CsvTopCustomers(custVm.Data, filter);
                    filename   = $"top_customers_{CsvDateRange(filter)}.csv";
                    break;

                case "topproducts":
                    var prodVm = await _reportService.GetTopProductsAsync(filter, top);
                    csv        = CsvTopProducts(prodVm.Data, filter);
                    filename   = $"top_products_{CsvDateRange(filter)}.csv";
                    break;

                default: // salesperproduct
                    var spVm   = await _reportService.GetSalesPerProductAsync(filter);
                    csv        = CsvProduct(spVm.Data, filter);
                    filename   = $"sales_per_product_{CsvDateRange(filter)}.csv";
                    break;
            }

            return File(Encoding.UTF8.GetBytes(csv), "text/csv", filename);
        }

        // ── CSV builders ──────────────────────────────────────────────────
        private static string CsvDateRange(SalesReportFilterViewModel f)
            => $"{f.EffectiveFrom:yyyyMMdd}_to_{f.EffectiveTo:yyyyMMdd}";

        private static string CsvProduct(List<ProductSalesReportViewModel> d,
            SalesReportFilterViewModel f)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"# Sales Per Product — {f.EffectiveFrom:dd MMM yyyy} to {f.EffectiveTo:dd MMM yyyy}");
            sb.AppendLine("Rank,Product,Category,Qty Sold,Revenue (PKR),Avg Unit Price (PKR)");
            foreach (var r in d)
                sb.AppendLine($"{r.Rank},\"{r.ProductName}\",\"{r.CategoryName}\"," +
                              $"{r.TotalQuantitySold},{r.TotalRevenue:F2},{r.AvgUnitPrice:F2}");
            return sb.ToString();
        }

        private static string CsvCategory(List<CategorySalesReportViewModel> d,
            SalesReportFilterViewModel f)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"# Sales Per Category — {f.EffectiveFrom:dd MMM yyyy} to {f.EffectiveTo:dd MMM yyyy}");
            sb.AppendLine("Rank,Category,Products Sold,Qty Sold,Revenue (PKR)");
            foreach (var r in d)
                sb.AppendLine($"{r.Rank},\"{r.CategoryName}\",{r.ProductCount}," +
                              $"{r.TotalQuantitySold},{r.TotalRevenue:F2}");
            return sb.ToString();
        }

        private static string CsvTopCustomers(List<TopCustomerViewModel> d,
            SalesReportFilterViewModel f)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"# Top Customers — {f.EffectiveFrom:dd MMM yyyy} to {f.EffectiveTo:dd MMM yyyy}");
            sb.AppendLine("Rank,Customer,Area,Invoices,Total Spend (PKR),Avg Order (PKR),Last Purchase");
            foreach (var r in d)
                sb.AppendLine($"{r.Rank},\"{r.CustomerName}\",\"{r.AreaName}\"," +
                              $"{r.InvoiceCount},{r.TotalSpend:F2},{r.AvgOrderValue:F2}," +
                              $"{(r.LastPurchase.HasValue ? r.LastPurchase.Value.ToString("dd MMM yyyy") : "")}");
            return sb.ToString();
        }

        private static string CsvTopProducts(List<TopProductViewModel> d,
            SalesReportFilterViewModel f)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"# Top Products — {f.EffectiveFrom:dd MMM yyyy} to {f.EffectiveTo:dd MMM yyyy}");
            sb.AppendLine("Rank,Product,Category,Qty Sold,Revenue (PKR),Profit (PKR),Margin %");
            foreach (var r in d)
                sb.AppendLine($"{r.Rank},\"{r.ProductName}\",\"{r.CategoryName}\"," +
                              $"{r.TotalQuantitySold},{r.TotalRevenue:F2},{r.TotalProfit:F2},{r.MarginPct:F1}");
            return sb.ToString();
        }
    }
}
