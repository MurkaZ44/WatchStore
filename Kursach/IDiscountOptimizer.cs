namespace Kursach;

public interface IDiscountOptimizer
{
    double CalculateOptimalDiscount(DiscountProductDto product, double minDiscount, double maxDiscount);
    int PredictSales(DiscountProductDto product, double discount);
}