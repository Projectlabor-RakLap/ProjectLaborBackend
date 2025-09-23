using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;

namespace ProjectLaborBackend.Entities
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<StockChange> StockChanges { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            DotNetEnv.Env.Load();
            string connectionString = DotNetEnv.Env.GetString("DB_CONNECTION_STRING");
            // string connectionString = "Server=MSI\\LOCALDB;Database=RakLapDb;Trusted_Connection=True;TrustServerCertificate=True;"; Márk connection string
            optionsBuilder.UseSqlServer(connectionString);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasConversion<string>();

            modelBuilder.Entity<User>()
                .HasMany(u => u.Warehouses)
                .WithMany(w => w.Users);

            modelBuilder.Entity<Warehouse>()
                .HasMany(w => w.Stock)
                .WithOne(s => s.Warehouse);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.Stocks)
                .WithOne(s => s.Product);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.StockChanges)
                .WithOne(sc => sc.Product);

            modelBuilder.Entity<Stock>()
                .HasOne(s => s.Product)
                .WithMany(p => p.Stocks);
        }
    }

    public enum Role { Admin, Manager, Analist}

    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(75)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(75)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }
        public Role Role { get; set; }
        public ICollection<Warehouse> Warehouses { get; set; }
    }

    public class Warehouse
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        [StringLength(200)]
        public string Location { get; set; }
        public ICollection<User> Users { get; set; }
        public ICollection<Stock> Stock { get; set; }
    }

    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(20)]
        public string EAN { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(500)]
        public string Description { get; set; }
        public string Image { get; set; }
        public ICollection<Stock> Stocks { get; set; }
        public ICollection<StockChange> StockChanges { get; set; }
    }

    public class Stock
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int StockInWarehouse { get; set; }
        [Required]
        public int StockInStore { get; set; }
        [Required]
        public int WarehouseCapacity { get; set; }
        [Required]
        public int StoreCapacity { get; set; }

        [Required]
        public double Price { get; set; }
        [Required]
        [StringLength(50)]
        public string Currency { get; set; }
        public int WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
    }

    public class StockChange
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int Quantity { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime ChangeDate { get; set; } = DateTime.Now;
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
