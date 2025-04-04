using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class InventoryContext : DbContext
    {
        public InventoryContext(DbContextOptions<InventoryContext> options)
            : base(options)
        {
        }

        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<WarehouseItem> WarehouseItems { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Warehouse
            modelBuilder.Entity<Warehouse>()
                .HasIndex(w => w.Name)
                .IsUnique();

            modelBuilder.Entity<Warehouse>()
                .Property(w => w.Name)
                .IsRequired();

            modelBuilder.Entity<Warehouse>()
                .Property(w => w.Address)
                .IsRequired();

            modelBuilder.Entity<Warehouse>()
                .Property(w => w.City)
                .IsRequired();

            modelBuilder.Entity<Warehouse>()
                .Property(w => w.Country)
                .IsRequired();

            // Configure WarehouseItem
            modelBuilder.Entity<WarehouseItem>()
                .Property(wi => wi.ItemName)
                .IsRequired();

            modelBuilder.Entity<WarehouseItem>()
                .Property(wi => wi.Quantity)
                .HasDefaultValue(1);

            modelBuilder.Entity<WarehouseItem>()
                .Property(wi => wi.CostPrice)
                .IsRequired();

            modelBuilder.Entity<WarehouseItem>()
                .HasIndex(wi => new { wi.WarehouseId, wi.ItemName })
                .IsUnique();

            // Configure User
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.FullName)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.PasswordHash)
                .IsRequired();

            // Configure Role
            modelBuilder.Entity<Role>()
                .Property(r => r.Name)
                .IsRequired();

            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                Email = "admin@happywarehouse.com",
                FullName = "Admin User",
                PasswordHash = "sD3fPKLnFKZUjnSV4qA/XoJOqsmDfNfxWcZ7kPtLc0I=",
                RoleId = 1,
                Active = true
            });

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "Management" },
                new Role { Id = 3, Name = "Auditor" }
            );
        }
    
}
}
