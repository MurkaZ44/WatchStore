using Kursach.Model.Repositories;

namespace Kursach.Model.Models;

public class Sale : ISale
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int ClientId { get; set; }
    public DateTime Date { get; set; }
    public decimal Price { get; set; }
    public string PaymentType { get; set; } = string.Empty;
    public int Quantity { get; set; }
    
    // Навигационные свойства для EF Core
    public Product? Product { get; set; }
    public Client? Client { get; set; }
    
    // Связь с продавцом
    public int SellerId { get; set; }
    public Seller? Seller { get; set; }
}