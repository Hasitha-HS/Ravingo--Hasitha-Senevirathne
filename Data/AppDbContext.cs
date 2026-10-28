using Microsoft.EntityFrameworkCore;
using Ravigno.StockTransfer.Api.Models;

namespace Ravigno.StockTransfer.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<BranchStock> BranchStocks => Set<BranchStock>();
    public DbSet<StockTransferRecord> StockTransfers => Set<StockTransferRecord>();
    public DbSet<StockTransferLine> StockTransferLines => Set<StockTransferLine>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Branch>().HasData(
            new Branch { Id = 1, Name = "Central Warehouse", Location = "Colombo Central" },
            new Branch { Id = 2, Name = "Colombo Branch", Location = "Colombo 03" },
            new Branch { Id = 3, Name = "Kandy Branch", Location = "Kandy City" },
            new Branch { Id = 4, Name = "Galle Branch", Location = "Galle Fort" }
        );

        modelBuilder.Entity<Item>().HasData(
            new Item { Id = 1, Sku = "SKU-LAP-01", Name = "Dell Latitude 5420", UnitPrice = 185000m },
            new Item { Id = 2, Sku = "SKU-MON-02", Name = "Dell 24\" Monitor", UnitPrice = 45000m },
            new Item { Id = 3, Sku = "SKU-KEY-03", Name = "Logitech MX Keys", UnitPrice = 28000m },
            new Item { Id = 4, Sku = "SKU-MOU-04", Name = "Logitech MX Master 3S", UnitPrice = 22000m },
            new Item { Id = 5, Sku = "SKU-HDM-05", Name = "HDMI Cable 2m", UnitPrice = 3500m }
        );

        modelBuilder.Entity<BranchStock>().HasData(
            new BranchStock { Id = 1, BranchId = 1, ItemId = 1, Quantity = 50 },
            new BranchStock { Id = 2, BranchId = 1, ItemId = 2, Quantity = 100 },
            new BranchStock { Id = 3, BranchId = 1, ItemId = 3, Quantity = 80 },
            new BranchStock { Id = 4, BranchId = 1, ItemId = 4, Quantity = 75 },
            new BranchStock { Id = 5, BranchId = 1, ItemId = 5, Quantity = 200 },

            new BranchStock { Id = 6, BranchId = 2, ItemId = 1, Quantity = 10 },
            new BranchStock { Id = 7, BranchId = 2, ItemId = 2, Quantity = 15 },

            new BranchStock { Id = 8, BranchId = 3, ItemId = 1, Quantity = 5 },
            new BranchStock { Id = 9, BranchId = 3, ItemId = 3, Quantity = 8 }
        );
    }
}