using Xunit;
using Kursach;
using Kursach.Converters;
using Kursach.Model.Models;
using Kursach.ViewModel;
using System.Globalization;
using System.Windows.Media.Imaging;

namespace TestProject1
{
    // Тесты для SimpleDiscountOptimizer (Бизнес-логика скидок)
    public class SimpleDiscountOptimizerTests
    {
        private readonly SimpleDiscountOptimizer _optimizer = new();

        [Theory]
        [InlineData(3, false, 0.05, 0.30, 0.30)] // low sales, demandFactor=1.0 -> max
        [InlineData(10, false, 0.05, 0.30, 0.20)] // medium sales, demandFactor=0.6 -> 0.05 + 0.25*0.6 = 0.20
        [InlineData(25, false, 0.05, 0.30, 0.12)] // high sales, demandFactor=0.3 -> 0.05 + 0.25*0.3 = 0.125 -> 0.12 (Banker's rounding)
        public void CalculateOptimalDiscount_SalesCount_VariesDiscount(int salesCount, bool isPremium, double min, double max, double expected)
        {
            // Arrange
            var product = new DiscountProductDto 
            { 
                TotalSalesCount = salesCount, 
                IsPremiumBrand = isPremium 
            };

            // Act
            var result = _optimizer.CalculateOptimalDiscount(product, min, max);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void CalculateOptimalDiscount_PremiumBrand_CapsMaxDiscount()
        {
            // Arrange
            var product = new DiscountProductDto 
            { 
                TotalSalesCount = 3, 
                IsPremiumBrand = true 
            };
            double min = 0.05;
            double max = 0.30; 

            // Act
            var result = _optimizer.CalculateOptimalDiscount(product, min, max);

            // Assert
            // Premium cap is 0.15. Result = 0.05 + (0.15 - 0.05) * 1.0 = 0.15
            Assert.Equal(0.15, result);
        }

        [Theory]
        [InlineData(10, 0.1, 5)] // baseDemand = 4, demand = 4 * (1 + 0.3) = 5.2 -> 5
        [InlineData(0, 0.0, 3)]  // baseDemand = 3, demand = 3 * 1 = 3
        [InlineData(100, 0.5, 32)] // baseDemand = 13, demand = 13 * (1 + 1.5) = 32.5 -> 32 (Banker's rounding)
        public void PredictSales_ReturnsCorrectValue(int salesCount, double discount, int expected)
        {
            // Arrange
            var product = new DiscountProductDto { TotalSalesCount = salesCount };

            // Act
            var result = _optimizer.PredictSales(product, discount);

            // Assert
            Assert.Equal(expected, result);
        }
    }

    // Тесты для NullToVisibilityConverter
    public class NullToVisibilityConverterTests
    {
        private readonly NullToVisibilityConverter _converter = new();

        [Fact]
        public void Convert_NullValue_ReturnsCollapsed()
        {
            // Arrange
            object value = null;

            // Act
            var result = _converter.Convert(value, typeof(object), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.Equal(System.Windows.Visibility.Collapsed, result);
        }

        [Fact]
        public void Convert_NotNullValue_ReturnsVisible()
        {
            // Arrange
            object value = new object();

            // Act
            var result = _converter.Convert(value, typeof(object), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.Equal(System.Windows.Visibility.Visible, result);
        }
    }

    // Тесты для CurrentPriceConverter
    public class CurrentPriceConverterTests
    {
        private readonly CurrentPriceConverter _converter = new();

        [Fact]
        public void Convert_ProductWithDiscount_ReturnsDiscountedPrice()
        {
            // Arrange
            var product = new Product 
            { 
                Price = 1000m, 
                DiscountedPrice = 800m 
            };

            // Act
            var result = _converter.Convert(product, typeof(string), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.Equal("800.00", result);
        }

        [Fact]
        public void Convert_ProductWithoutDiscount_ReturnsOriginalPrice()
        {
            // Arrange
            var product = new Product 
            { 
                Price = 1500.50m, 
                DiscountedPrice = null 
            };

            // Act
            var result = _converter.Convert(product, typeof(string), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.Equal("1500.50", result);
        }

        [Fact]
        public void Convert_NonProductValue_ReturnsZero()
        {
            // Arrange
            object value = "not a product";

            // Act
            var result = _converter.Convert(value, typeof(string), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.Equal("0.00", result);
        }
    }

    // Тесты для ImagePathConverter
    public class ImagePathConverterTests
    {
        private readonly ImagePathConverter _converter = new();

        [Fact]
        public void Convert_NullValue_ReturnsNull()
        {
            // Arrange
            object? value = null;

            // Act
            var result = _converter.Convert(value, typeof(BitmapImage), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Convert_EmptyString_ReturnsNull()
        {
            // Arrange
            object value = string.Empty;

            // Act
            var result = _converter.Convert(value, typeof(BitmapImage), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Convert_NonExistentFile_ReturnsNull()
        {
            // Arrange
            object value = "non_existent_image.jpg";

            // Act
            var result = _converter.Convert(value, typeof(BitmapImage), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.Null(result);
        }
    }

    // Тесты для моделей данных (DTO & Models)
    public class ModelTests
    {
        [Fact]
        public void DiscountProductDto_CanBeInitialized()
        {
            // Arrange & Act
            var dto = new DiscountProductDto
            {
                Id = 1,
                DisplayName = "Test Watch",
                Brand = "Rolex",
                Type = "Mechanical",
                IsPremiumBrand = true,
                Price = 5000m,
                QuantityInStock = 10,
                TotalSalesCount = 5,
                AppliedDiscount = 0.1,
                DiscountedPrice = 4500m
            };

            // Assert
            Assert.Equal(1, dto.Id);
            Assert.True(dto.IsPremiumBrand);
            Assert.Equal("Test Watch", dto.DisplayName);
        }

        [Fact]
        public void Product_Model_CanBeInitialized()
        {
            // Arrange & Act
            var product = new Product
            {
                Id = 2,
                Brand = "Casio",
                Model = "G-Shock",
                Price = 200m,
                QuantityInStock = 50,
                IsPremiumBrand = false,
                DiscountedPrice = 180m,
                AppliedDiscount = 0.1
            };

            // Assert
            Assert.Equal(2, product.Id);
            Assert.False(product.IsPremiumBrand);
            Assert.Equal(180m, product.DiscountedPrice);
        }

        [Fact]
        public void Client_Model_CanBeInitialized()
        {
            // Arrange & Act
            var client = new Client
            {
                Id = 1,
                FullName = "Ivanov Ivan",
                Phone = "+123456789",
                Email = "ivanov@test.com"
            };

            // Assert
            Assert.Equal(1, client.Id);
            Assert.Equal("Ivanov Ivan", client.FullName);
        }

        [Fact]
        public void Sale_Model_CanBeInitialized()
        {
            // Arrange & Act
            var sale = new Sale
            {
                Id = 1,
                ProductId = 10,
                ClientId = 5,
                Price = 500m,
                Quantity = 2,
                PaymentType = "Card"
            };

            // Assert
            Assert.Equal(1, sale.Id);
            Assert.Equal(500m, sale.Price);
            Assert.Equal(2, sale.Quantity);
        }
    }

    // Тесты для ProductSaleData
    public class ProductSaleDataTests
    {
        [Fact]
        public void ProductSaleData_CanBeInitialized()
        {
            // Arrange & Act
            var data = new ProductSaleData
            {
                ProductModel = "Test Model",
                TotalQuantity = 10m,
                TotalRevenue = 5000.00m
            };

            // Assert
            Assert.Equal("Test Model", data.ProductModel);
            Assert.Equal(10m, data.TotalQuantity);
            Assert.Equal(5000.00m, data.TotalRevenue);
        }
    }
}