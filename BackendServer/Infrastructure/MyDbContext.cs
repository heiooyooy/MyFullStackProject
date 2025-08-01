using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SharedModule;

namespace Infrastructure;

public class MyDbContext : IdentityDbContext<ApplicationUser>
{
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Seed some data for demonstration
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 1, Name = "Laptop Pro", Price = 1200.00m, Description = "High-performance laptop.",
                ImageUrl = "images/products/c1368755-0029-41b8-9ac2-d786a15793cd-laptop.jpg"
            },
            new Product
            {
                Id = 2, Name = "Mechanical Keyboard", Price = 150.00m, Description = "Tactile typing experience.",
                ImageUrl = "images/products/2ab9e161-f82c-402b-ba60-3c65b00f88f2-headphones.jpg"
            },
            new Product
            {
                Id = 3, Name = "Wireless Mouse", Price = 50.00m, Description = "Ergonomic design.",
                ImageUrl = "images/products/c1368755-0029-41b8-9ac2-d786a15793cd-laptop.jpg"
            }
        );
    }
}