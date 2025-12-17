using Kursach.Model.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Kursach;

public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<WarrantyService> WarrantyServices { get; set; }
    public DbSet<Seller> Sellers { get; set; }
    
    public AppDbContext()
    {
        // Database.EnsureCreated(); // Убираем, чтобы не мешать миграциям в будущем
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite("Data Source=watchstore.db");
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Настройка связей
        modelBuilder.Entity<Sale>()
            .HasOne(s => s.Product)
            .WithMany(p => p.Sales)
            .HasForeignKey(s => s.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
            
        modelBuilder.Entity<Sale>()
            .HasOne(s => s.Client)
            .WithMany()
            .HasForeignKey(s => s.ClientId)
            .OnDelete(DeleteBehavior.Restrict);
            
        modelBuilder.Entity<WarrantyService>()
            .HasOne(w => w.Product)
            .WithMany(p => p.WarrantyServices)
            .HasForeignKey(w => w.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
            
        modelBuilder.Entity<WarrantyService>()
            .HasOne(w => w.Client)
            .WithMany()
            .HasForeignKey(w => w.ClientId)
            .OnDelete(DeleteBehavior.Restrict);
            
        modelBuilder.Entity<Sale>()
            .HasOne(s => s.Seller)
            .WithMany()
            .HasForeignKey(s => s.SellerId)
            .OnDelete(DeleteBehavior.Restrict);
            
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Supplier)
            .WithMany()
            .HasForeignKey(p => p.SupplierId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
