namespace Kursach;

public class DiscountProductDto
{
    public int Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsPremiumBrand { get; set; }
    public decimal Price { get; set; }
    public int QuantityInStock { get; set; }

    // Простая оценка «возраста» товара по продажам:
    public int TotalSalesCount { get; set; }
}