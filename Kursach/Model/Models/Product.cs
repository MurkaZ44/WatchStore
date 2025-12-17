using Kursach.Model.Repositories;

namespace Kursach.Model.Models;

public class Product : IProduct
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public bool IsPremiumBrand { get; set; } // New property
    public decimal Price { get; set; }
    public int QuantityInStock { get; set; }
    public int WarrantyPeriod { get; set; }
    public string ImagePath { get; set; } = string.Empty;

    public int? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }
    // Новые поля для скидки
    public decimal? DiscountedPrice { get; set; }      // цена со скидкой
    public double? AppliedDiscount { get; set; }   
    
    // Навигационные свойства для EF Core
    public ICollection<Sale>? Sales { get; set; }
    public ICollection<WarrantyService>? WarrantyServices { get; set; }
}