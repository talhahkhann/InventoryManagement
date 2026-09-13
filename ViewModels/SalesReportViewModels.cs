using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.ViewModels
{
    // ── Filter bar ────────────────────────────────────────────────────────
    public class SalesReportFilterViewModel
    {
        [DataType(DataType.Date)]
        [Display(Name = "From")]
        public DateTime? From { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "To")]
        public DateTime? To { get; set; }

        /// <summary>Convenience: effective start of range (defaults to 1 year ago).</summary>
        public DateTime EffectiveFrom =>
            From?.Date ?? DateTime.Today.AddYears(-1);

        /// <summary>Convenience: effective end of range (defaults to today end-of-day).</summary>
        public DateTime EffectiveTo =>
            (To?.Date ?? DateTime.Today).AddDays(1).AddSeconds(-1);

        public bool HasFilter => From.HasValue || To.HasValue;
    }

    // ── Sales per product ─────────────────────────────────────────────────
    public class ProductSalesReportViewModel
    {
        public int     Rank              { get; set; }
        public string  ProductName       { get; set; } = string.Empty;
        public string  CategoryName      { get; set; } = string.Empty;
        public int     TotalQuantitySold { get; set; }
        public decimal TotalRevenue      { get; set; }
        public decimal AvgUnitPrice      { get; set; }
    }

    // ── Sales per category ────────────────────────────────────────────────
    public class CategorySalesReportViewModel
    {
        public int     Rank              { get; set; }
        public string  CategoryName      { get; set; } = string.Empty;
        public int     TotalQuantitySold { get; set; }
        public decimal TotalRevenue      { get; set; }
        public int     ProductCount      { get; set; }   // distinct products sold
    }

    // ── Top customers ─────────────────────────────────────────────────────
    public class TopCustomerViewModel
    {
        public int     Rank          { get; set; }
        public int     CustomerId    { get; set; }
        public string  CustomerName  { get; set; } = string.Empty;
        public string  AreaName      { get; set; } = string.Empty;
        public int     InvoiceCount  { get; set; }
        public decimal TotalSpend    { get; set; }
        public decimal AvgOrderValue { get; set; }
        public DateTime? LastPurchase { get; set; }
    }

    // ── Top products ──────────────────────────────────────────────────────
    public class TopProductViewModel
    {
        public int     Rank              { get; set; }
        public int     ProductId         { get; set; }
        public string  ProductName       { get; set; } = string.Empty;
        public string  CategoryName      { get; set; } = string.Empty;
        public int     TotalQuantitySold { get; set; }
        public decimal TotalRevenue      { get; set; }
        public decimal TotalProfit       { get; set; }
        public decimal MarginPct         { get; set; }
    }

    // ── Wrapper passed to all report views ───────────────────────────────
    public class SalesReportPageViewModel<T>
    {
        public SalesReportFilterViewModel Filter { get; set; } = new();
        public List<T>                    Data   { get; set; } = new();

        // Available years for the year-picker shortcut
        public List<int> AvailableYears { get; set; } = new();
    }
}
