
using BuyoraModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BuyoraDataAccess.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext()
        {
        }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Category { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<ProductImage> ProductImage { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                .Property(x => x.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Product>()
                .Property(x => x.DiscountPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(d => d.Product).WithMany()
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                  .HasOne(o => o.ApplicationUser).WithMany()
                  .HasForeignKey(o => o.ApplicationUserId)
                  .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>().Property(o => o.TotalAmount).HasPrecision(18, 2);
            modelBuilder.Entity<OrderDetail>().Property(d => d.UnitPrice).HasPrecision(18, 2);

            modelBuilder.Entity<Order>().HasIndex(o => new { o.ApplicationUserId, o.OrderDate });

        }
    }


}
