namespace InventoryManagement.ViewModels
{
    // ── One invoice row in the history table ──────────────────────────────
    public class CustomerInvoiceRowViewModel
    {
        public int      Id          { get; set; }
        public DateTime Date        { get; set; }
        public int      ItemCount   { get; set; }
        public decimal  Total       { get; set; }

        // top product name on this invoice (by quantity)
        public string   TopProduct  { get; set; } = string.Empty;
    }

    // ── Full customer history page model ─────────────────────────────────
    public class CustomerHistoryViewModel
    {
        // ── Profile ───────────────────────────────────────────────────
        public int     Id          { get; set; }
        public string  Name        { get; set; } = string.Empty;
        public string  Email       { get; set; } = string.Empty;
        public string  Phone       { get; set; } = string.Empty;
        public string  Address     { get; set; } = string.Empty;
        public string  City        { get; set; } = string.Empty;
        public string  Country     { get; set; } = string.Empty;
        public string  AreaName    { get; set; } = string.Empty;

        // ── Computed stats (derived in service) ───────────────────────
        public int      TotalInvoices    { get; set; }
        public decimal  TotalSpend       { get; set; }
        public decimal  AvgOrderValue    { get; set; }
        public DateTime? FirstPurchase   { get; set; }
        public DateTime? LastPurchase    { get; set; }

        /// <summary>Most-ordered product name across all invoices.</summary>
        public string   TopProduct       { get; set; } = string.Empty;

        /// <summary>Total units of that top product bought.</summary>
        public int      TopProductUnits  { get; set; }

        // ── Invoice list ─────────────────────────────────────────────
        public List<CustomerInvoiceRowViewModel> Invoices { get; set; } = new();
    }
}
