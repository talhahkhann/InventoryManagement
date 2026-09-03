using InventoryManagement.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // ── Existing tables ───────────────────────────────────────────
        public DbSet<Category>             Categories             { get; set; }
        public DbSet<Product>              Products               { get; set; }
        public DbSet<Customer>             Customers              { get; set; }
        public DbSet<Invoice>              Invoices               { get; set; }
        public DbSet<InvoiceItem>          InvoiceItems           { get; set; }
        public DbSet<CustomerProductPrice> CustomerProductPrices  { get; set; }
        public DbSet<StockAlert>           StockAlerts            { get; set; }
        public DbSet<Area>                 Areas                  { get; set; }
        public DbSet<ProfitRecord>         ProfitRecords          { get; set; }

        // ── Purchase Order module ─────────────────────────────────────
        public DbSet<Supplier>           Suppliers           { get; set; }
        public DbSet<PurchaseOrder>      PurchaseOrders      { get; set; }
        public DbSet<PurchaseOrderItem>  PurchaseOrderItems  { get; set; }

        // ── Audit Log ─────────────────────────────────────────────────
        public DbSet<AuditLog>           AuditLogs           { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Area → Customers (1:many)
            builder.Entity<Customer>()
                .HasOne(c => c.Area)
                .WithMany(a => a.Customers)
                .HasForeignKey(c => c.AreaId);

            // Unique index on customer-product pricing
            builder.Entity<CustomerProductPrice>()
                .HasIndex(cpp => new { cpp.CustomerId, cpp.ProductId })
                .IsUnique();

            // PurchaseOrder → Supplier (many:1)
            builder.Entity<PurchaseOrder>()
                .HasOne(po => po.Supplier)
                .WithMany(s => s.PurchaseOrders)
                .HasForeignKey(po => po.SupplierId)
                .OnDelete(DeleteBehavior.Restrict); // don't cascade-delete orders when supplier deleted

            // PurchaseOrderItem → PurchaseOrder (many:1) — cascade delete items with order
            builder.Entity<PurchaseOrderItem>()
                .HasOne(poi => poi.PurchaseOrder)
                .WithMany(po => po.Items)
                .HasForeignKey(poi => poi.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // PurchaseOrderItem → Product (many:1) — restrict, keep order history
            builder.Entity<PurchaseOrderItem>()
                .HasOne(poi => poi.Product)
                .WithMany()
                .HasForeignKey(poi => poi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
