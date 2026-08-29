// ProfitViewModels.cs — All ViewModels for the Profit Calculator module

namespace InventoryManagement.ViewModels
{
    // ── Filter / Request ────────────────────────────────────────────────────

    /// <summary>
    /// Carries the user's selected date range and grouping for profit queries.
    /// </summary>
    public class ProfitFilterViewModel
    {
        public int?   Year      { get; set; }
        public int?   Month     { get; set; }
        public DateTime? From   { get; set; }
        public DateTime? To     { get; set; }

        // Convenience: resolve effective date range
        public DateTime EffectiveFrom =>
            From ?? (Year.HasValue && Month.HasValue
                ? new DateTime(Year.Value, Month.Value, 1)
                : Year.HasValue
                    ? new DateTime(Year.Value, 1, 1)
                    : new DateTime(DateTime.Today.Year, 1, 1));

        public DateTime EffectiveTo =>
            To ?? (Year.HasValue && Month.HasValue
                ? new DateTime(Year.Value, Month.Value,
                    DateTime.DaysInMonth(Year.Value, Month.Value), 23, 59, 59)
                : Year.HasValue
                    ? new DateTime(Year.Value, 12, 31, 23, 59, 59)
                    : new DateTime(DateTime.Today.Year, 12, 31, 23, 59, 59));
    }

    // ── Summary Cards (Dashboard) ───────────────────────────────────────────

    /// <summary>
    /// Top-level KPIs shown on the profit dashboard.
    /// </summary>
    public class ProfitSummaryViewModel
    {
        public decimal TotalRevenue      { get; set; }
        public decimal TotalCost         { get; set; }
        public decimal TotalProfit       { get; set; }
        public decimal ProfitMarginPct   => TotalRevenue == 0 ? 0
                                          : Math.Round(TotalProfit / TotalRevenue * 100, 2);
        public int     TotalInvoices     { get; set; }
        public int     TotalUnitsSold    { get; set; }

        // Period comparison (vs previous equivalent period)
        public decimal PrevRevenue       { get; set; }
        public decimal PrevProfit        { get; set; }
        public decimal RevenueDeltaPct   => PrevRevenue == 0 ? 0
                                          : Math.Round((TotalRevenue - PrevRevenue) / PrevRevenue * 100, 1);
        public decimal ProfitDeltaPct    => PrevProfit == 0 ? 0
                                          : Math.Round((TotalProfit - PrevProfit) / PrevProfit * 100, 1);

        // For the chart data endpoint
        public List<ProfitByMonthViewModel>    MonthlyBreakdown  { get; set; } = new();
        public List<ProfitByProductViewModel>  TopProducts       { get; set; } = new();

        // Filter context (what period this summary covers)
        public ProfitFilterViewModel Filter { get; set; } = new();
        public List<int> AvailableYears  { get; set; } = new();
    }

    // ── Monthly Breakdown ───────────────────────────────────────────────────

    public class ProfitByMonthViewModel
    {
        public int     Year              { get; set; }
        public int     Month             { get; set; }
        public string  MonthName         => new DateTime(Year, Month, 1).ToString("MMM yyyy");
        public decimal Revenue           { get; set; }
        public decimal Cost              { get; set; }
        public decimal Profit            { get; set; }
        public decimal MarginPct         => Revenue == 0 ? 0
                                          : Math.Round(Profit / Revenue * 100, 2);
        public int     InvoiceCount      { get; set; }
        public int     UnitsSold         { get; set; }
    }

    // ── Yearly Breakdown ────────────────────────────────────────────────────

    public class ProfitByYearViewModel
    {
        public int     Year              { get; set; }
        public decimal Revenue           { get; set; }
        public decimal Cost              { get; set; }
        public decimal Profit            { get; set; }
        public decimal MarginPct         => Revenue == 0 ? 0
                                          : Math.Round(Profit / Revenue * 100, 2);
        public int     InvoiceCount      { get; set; }
        public int     UnitsSold         { get; set; }
    }

    // ── Per-Product Breakdown ───────────────────────────────────────────────

    public class ProfitByProductViewModel
    {
        public int     ProductId         { get; set; }
        public string  ProductName       { get; set; }
        public string  CategoryName      { get; set; }
        public decimal Revenue           { get; set; }
        public decimal Cost              { get; set; }
        public decimal Profit            { get; set; }
        public decimal MarginPct         => Revenue == 0 ? 0
                                          : Math.Round(Profit / Revenue * 100, 2);
        public int     UnitsSold         { get; set; }
        public decimal AvgSellingPrice   => UnitsSold == 0 ? 0
                                          : Math.Round(Revenue / UnitsSold, 2);
        public decimal AvgCostPrice      => UnitsSold == 0 ? 0
                                          : Math.Round(Cost    / UnitsSold, 2);
    }

    // ── Per-Customer Breakdown ──────────────────────────────────────────────

    public class ProfitByCustomerViewModel
    {
        public int     CustomerId        { get; set; }
        public string  CustomerName      { get; set; }
        public string  AreaName          { get; set; }
        public decimal Revenue           { get; set; }
        public decimal Cost              { get; set; }
        public decimal Profit            { get; set; }
        public decimal MarginPct         => Revenue == 0 ? 0
                                          : Math.Round(Profit / Revenue * 100, 2);
        public int     InvoiceCount      { get; set; }
        public int     UnitsSold         { get; set; }
    }

    // ── Per-Category Breakdown ──────────────────────────────────────────────

    public class ProfitByCategoryViewModel
    {
        public int     CategoryId        { get; set; }
        public string  CategoryName      { get; set; }
        public decimal Revenue           { get; set; }
        public decimal Cost              { get; set; }
        public decimal Profit            { get; set; }
        public decimal MarginPct         => Revenue == 0 ? 0
                                          : Math.Round(Profit / Revenue * 100, 2);
        public int     UnitsSold         { get; set; }
    }

    // ── Profit Dashboard (wraps all above) ──────────────────────────────────

    public class ProfitDashboardViewModel
    {
        public ProfitSummaryViewModel          Summary      { get; set; } = new();
        public List<ProfitByMonthViewModel>    ByMonth      { get; set; } = new();
        public List<ProfitByYearViewModel>     ByYear       { get; set; } = new();
        public List<ProfitByProductViewModel>  ByProduct    { get; set; } = new();
        public List<ProfitByCustomerViewModel> ByCustomer   { get; set; } = new();
        public List<ProfitByCategoryViewModel> ByCategory   { get; set; } = new();
        public ProfitFilterViewModel           Filter       { get; set; } = new();
        public List<int>                       AvailableYears { get; set; } = new();
    }
}
