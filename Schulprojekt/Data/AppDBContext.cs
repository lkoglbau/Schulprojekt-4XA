using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Schulprojekt.Models;

namespace Schulprojekt.Data
{
    public class AppDBContext : IdentityDbContext {

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<ShippingInfo> ShippingInfos { get; set; }

        public AppDBContext(DbContextOptions<AppDBContext> options)
       : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1:1 Beziehungen konfigurieren
            modelBuilder.Entity<Order>()
                .HasOne(o => o.ShippingInfo)
                .WithOne(s => s.Order)
                .HasForeignKey<ShippingInfo>(s => s.OrderId);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Payment)
                .WithOne(p => p.Order)
                .HasForeignKey<Payment>(p => p.OrderId);
        }
    }
}
