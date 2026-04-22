using GoodHamburger.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(e =>
        {
            e.HasKey(o => o.Id);
            e.Property(o => o.Subtotal).HasColumnType("decimal(10,2)");
            e.Property(o => o.DiscountAmount).HasColumnType("decimal(10,2)");
            e.Property(o => o.Total).HasColumnType("decimal(10,2)");
            e.Property(o => o.DiscountPercentage).HasColumnType("decimal(5,2)");
        });
    }
}
