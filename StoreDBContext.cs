using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Store.Models;

namespace Store
{
    public class StoreDBContext : IdentityDbContext<AppUser>
    {
        public StoreDBContext(DbContextOptions<StoreDBContext> options) : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductBarcode> Barcodes { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<AdminSetting> AdminSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ✅ Enforce uniqueness on Barcode
            modelBuilder.Entity<ProductBarcode>()
                .HasIndex(p => p.Barcode)
                .IsUnique();
        }
    }
}
