namespace Kursach;

public class SimpleDiscountOptimizer : IDiscountOptimizer
{
    public double CalculateOptimalDiscount(DiscountProductDto product, double minDiscount, double maxDiscount)
    {
        // Базовый коэффициент «проблемности» товара
        // Мало продаж → выше factor
        double demandFactor = product.TotalSalesCount switch
        {
            <= 5 => 1.0,   // плохо продается
            <= 20 => 0.6,
            _ => 0.3       // хорошо продается
        };

        // Премиум бренды — ограничим максимальную скидку
        double premiumCap = product.IsPremiumBrand ? 0.15 : maxDiscount;

        double targetMax = Math.Min(maxDiscount, premiumCap);
        double discount = minDiscount + (targetMax - minDiscount) * demandFactor;

        return Math.Round(discount, 2);
    }

    public int PredictSales(DiscountProductDto product, double discount)
    {
        // Очень упрощенный прогноз: базовый спрос + влияние скидки
        double baseDemand = product.TotalSalesCount / 10.0 + 3; // чтобы не было нуля
        double demand = baseDemand * (1 + discount * 3);
        return Math.Max(0, (int)Math.Round(demand));
    }
}

