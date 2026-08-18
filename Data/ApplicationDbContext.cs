using InventoryManagement.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<CustomerProductPrice> CustomerProductPrices { get; set; }

        public DbSet<Area> Areas { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //Define Relationship (1-Area -> Many Customers)
            builder.Entity<Customer>()
            .HasOne(c => c.Area)
            .WithMany(a => a.Customers)
            .HasForeignKey(c => c.AreaId);
            builder.Entity<CustomerProductPrice>()
           .HasIndex(cpp => new { cpp.CustomerId, cpp.ProductId })
           .IsUnique();
        }
    }
    
}