using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagement.Models
{
    /// <summary>
    /// Pre-computed profit snapshot written whenever an invoice is created or updated.
    /// One row per InvoiceItem — queries GROUP BY Year/Month/ProductId/CustomerId are instant.
    /// Formula: Profit = (SellingPrice - CostPrice) * Quantity
    /// </summary>
    public class ProfitRecord
    {
        [Key]
        public int Id { get; set; }

        // ── Source references ─────────────────────────────────────────────────
        [Required]
        public int InvoiceId     { get; set; }
        [ForeignKey(nameof(InvoiceId))]
        public Invoice Invoice   { get; set; }

        [Required]
        public int InvoiceItemId { get; set; }
        [ForeignKey(nameof(InvoiceItemId))]
        public InvoiceItem InvoiceItem { get; set; }

        [Required]
        public int ProductId     { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product   { get; set; }

        [Required]
        public int CustomerId    { get; set; }
        [ForeignKey(nameof(CustomerId))]
        public Customer Customer { get; set; }

        // ── Denormalised for fast GROUP BY ────────────────────────────────────
        public int    Year        { get; set; }   // Invoice date year
        public int    Month       { get; set; }   // Invoice date month
        public string ProductName { get; set; }   // snapshot — survives product renames
        public string CustomerName{ get; set; }   // snapshot
        public string CategoryName{ get; set; }   // snapshot
        public string AreaName    { get; set; }   // snapshot

        // ── Financials ────────────────────────────────────────────────────────
        public int     Quantity     { get; set; }
        public decimal SellingPrice { get; set; }  // per unit
        public decimal CostPrice    { get; set; }  // per unit
        public decimal Revenue      { get; set; }  // SellingPrice * Quantity
        public decimal Cost         { get; set; }  // CostPrice    * Quantity
        public decimal Profit       { get; set; }  // Revenue - Cost
        public decimal MarginPct    { get; set; }  // Profit / Revenue * 100

        // ── Timestamp ─────────────────────────────────────────────────────────
        public DateTime InvoiceDate  { get; set; }
        public DateTime RecordedAt   { get; set; } = DateTime.UtcNow;
    }
}
