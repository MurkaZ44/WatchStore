using Xunit;
using Kursach;
using Kursach.Model.Models;
using Kursach.Model.Repositories;
using Kursach.ViewModel;
using Kursach.Services;
using Kursach.Converters;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Windows.Media.Imaging;

namespace TestProject1
{
    // Дополнительные тесты для репозиториев
    public class AdditionalRepositoryTests
    {
        private AppDbContext GetInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task ProductRepository_GetAll_ReturnsAllProducts()
        {
            using var context = GetInMemoryContext("TestDb_ProductAll");
            var repo = new ProductRepository(context);

            var product1 = new Product { Id = 1, Brand = "Casio", Model = "G-Shock", Price = 100m };
            var product2 = new Product { Id = 2, Brand = "Rolex", Model = "Submariner", Price = 5000m };

            await repo.AddAsync(product1);
            await repo.AddAsync(product2);

            var result = await repo.GetAllAsync();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task ProductRepository_Update_UpdatesProduct()
        {
            using var context = GetInMemoryContext("TestDb_ProductUpdate");
            var repo = new ProductRepository(context);

            var product = new Product { Id = 1, Brand = "Casio", Model = "G-Shock", Price = 100m };
            await repo.AddAsync(product);

            product.Price = 150m;
            await repo.UpdateAsync(product);

            var result = await repo.GetByIdAsync(1);
            Assert.NotNull(result);
            Assert.Equal(150m, result.Price);
        }

        [Fact]
        public async Task ProductRepository_Delete_DeletesProduct()
        {
            using var context = GetInMemoryContext("TestDb_ProductDelete");
            var repo = new ProductRepository(context);

            var product = new Product { Id = 1, Brand = "Casio", Model = "G-Shock", Price = 100m };
            await repo.AddAsync(product);

            var deleteResult = await repo.DeleteAsync(1);
            Assert.True(deleteResult);

            var result = await repo.GetByIdAsync(1);
            Assert.Null(result);
        }

        [Fact]
        public async Task ClientRepository_GetAll_ReturnsAllClients()
        {
            using var context = GetInMemoryContext("TestDb_ClientAll");
            var repo = new ClientRepository(context);

            var client1 = new Client { Id = 1, FullName = "Ivanov Ivan", Phone = "123" };
            var client2 = new Client { Id = 2, FullName = "Petrov Petr", Phone = "456" };

            await repo.AddAsync(client1);
            await repo.AddAsync(client2);

            var result = await repo.GetAllAsync();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task ClientRepository_Update_UpdatesClient()
        {
            using var context = GetInMemoryContext("TestDb_ClientUpdate");
            var repo = new ClientRepository(context);

            var client = new Client { Id = 1, FullName = "Ivanov Ivan", Phone = "123" };
            await repo.AddAsync(client);

            client.Phone = "999";
            await repo.UpdateAsync(client);

            var result = await repo.GetByIdAsync(1);
            Assert.NotNull(result);
            Assert.Equal("999", result.Phone);
        }

        [Fact]
        public async Task SupplierRepository_CRUD_Operations()
        {
            using var context = GetInMemoryContext("TestDb_SupplierCRUD");
            var repo = new SupplierRepository(context);

            var supplier = new Supplier
                { Id = 1, Name = "Supplier Inc.", ContactPerson = "John", Phone = "123", Email = "john@test.com" };
            await repo.AddAsync(supplier);

            var result = await repo.GetByIdAsync(1);
            Assert.NotNull(result);
            Assert.Equal("Supplier Inc.", result.Name);

            result.Name = "Updated Supplier";
            await repo.UpdateAsync(result);

            var updated = await repo.GetByIdAsync(1);
            Assert.Equal("Updated Supplier", updated.Name);

            var deleteResult = await repo.DeleteAsync(1);
            Assert.True(deleteResult);

            var deleted = await repo.GetByIdAsync(1);
            Assert.Null(deleted);
        }


        [Fact]
        public async Task WarrantyServiceRepository_CRUD_Operations()
        {
            using var context = GetInMemoryContext("TestDb_WarrantyCRUD");

            var product = new Product { Id = 1, Brand = "B", Model = "M", Price = 10m };
            var client = new Client { Id = 1, FullName = "C", Phone = "P" };
            context.Products.Add(product);
            context.Clients.Add(client);
            context.SaveChanges();

            var repo = new WarrantyServiceRepository(context);

            var warranty = new WarrantyService
            {
                Id = 1,
                ProductId = 1,
                ClientId = 1,
                IssueDescription = "Broken glass",
                Status = "Active",
                AcceptanceDate = DateTime.Now
            };
            await repo.AddAsync(warranty);

            var result = await repo.GetByIdAsync(1);
            Assert.NotNull(result);
            Assert.Equal("Active", result.Status);

            result.Status = "Completed";
            result.CompletionDate = DateTime.Now;
            await repo.UpdateAsync(result);

            var updated = await repo.GetByIdAsync(1);
            Assert.Equal("Completed", updated.Status);

            var all = await repo.GetAllAsync();
            Assert.Single(all);
        }

        [Fact]
        public async Task SaleHistoryRepository_GetSalesHistoryForProduct_ReturnsHistory()
        {
            using var context = GetInMemoryContext("TestDb_SaleHistory");

            var product = new Product { Id = 1, Brand = "B", Model = "M", Price = 10m };
            var client = new Client { Id = 1, FullName = "C", Phone = "P" };
            context.Products.Add(product);
            context.Clients.Add(client);
            context.SaveChanges();

            var sale1 = new Sale
                { Id = 1, ProductId = 1, ClientId = 1, Price = 10m, Quantity = 1, Date = DateTime.Now };
            var sale2 = new Sale
                { Id = 2, ProductId = 1, ClientId = 1, Price = 10m, Quantity = 2, Date = DateTime.Now };
            context.Sales.Add(sale1);
            context.Sales.Add(sale2);
            context.SaveChanges();

            var repo = new SaleHistoryRepository(context);
            var history = repo.GetSalesHistoryForProduct(1);

            Assert.Equal(2, history.Count());
        }
    }

    // Тесты для ViewModels
    public class ViewModelAdditionalTests
    {
        [Fact]
        public void ProductViewModel_EditMode_HasCorrectProperties()
        {
            var product = new Product
            {
                Id = 1,
                Brand = "Casio",
                Model = "G-Shock",
                Price = 100m,
                Type = "Digital",
                SerialNumber = "SN123",
                QuantityInStock = 10,
                WarrantyPeriod = 24,
                IsPremiumBrand = false,
                ImagePath = "test.jpg"
            };

            var vm = new ProductViewModel(product);

            Assert.True(vm.IsEditMode);
            Assert.Equal(1, vm.Id);
            Assert.Equal("Редактирование товара", vm.FormTitle);
            Assert.Equal("Casio", vm.Brand);
            Assert.Equal("G-Shock", vm.Model);
            Assert.Equal("100.00", vm.Price);
        }


        // Тесты для сервисов
        public class ServiceTests
        {
            [Fact]
            public void DialogService_CanBeInstantiated()
            {
                var service = new DialogService();
                Assert.NotNull(service);
            }

            [Fact]
            public void MockDialogService_ShowMessage_SetsWasCalled()
            {
                var service = new MockDialogService();

                Assert.False(service.WasCalled);

                service.ShowMessage("Test message");

                Assert.True(service.WasCalled);
            }

            [Fact]
            public void MockDialogService_ShowConfirmation_ReturnsTrue()
            {
                var service = new MockDialogService();

                var result = service.ShowConfirmation("Test confirmation");

                Assert.True(result);
            }

            [Fact]
            public void MockDialogService_ShowInputDialog_ReturnsTestInput()
            {
                var service = new MockDialogService();

                var result = service.ShowInputDialog("Test prompt");

                Assert.Equal("Test Input", result);
            }
        }

        // Тесты для дополнительных моделей и DTO
        public class ModelAdditionalTests
        {
            [Fact]
            public void Product_Model_HasAllProperties()
            {
                var product = new Product
                {
                    Id = 1,
                    Type = "Digital",
                    Model = "G-Shock",
                    Brand = "Casio",
                    SerialNumber = "SN123",
                    IsPremiumBrand = false,
                    Price = 100m,
                    QuantityInStock = 50,
                    WarrantyPeriod = 24,
                    ImagePath = "image.jpg",
                    SupplierId = 1,
                    DiscountedPrice = 90m,
                    AppliedDiscount = 0.1
                };

                Assert.Equal(1, product.Id);
                Assert.Equal("Digital", product.Type);
                Assert.Equal("G-Shock", product.Model);
                Assert.Equal("Casio", product.Brand);
                Assert.Equal("SN123", product.SerialNumber);
                Assert.False(product.IsPremiumBrand);
                Assert.Equal(100m, product.Price);
                Assert.Equal(50, product.QuantityInStock);
                Assert.Equal(24, product.WarrantyPeriod);
                Assert.Equal("image.jpg", product.ImagePath);
                Assert.Equal(1, product.SupplierId);
                Assert.Equal(90m, product.DiscountedPrice);
                Assert.Equal(0.1, product.AppliedDiscount);
            }

            [Fact]
            public void Sale_Model_HasAllProperties()
            {
                var sale = new Sale
                {
                    Id = 1,
                    ProductId = 10,
                    ClientId = 5,
                    Date = DateTime.Now,
                    Price = 500m,
                    PaymentType = "Card",
                    Quantity = 2,
                    SellerId = 1
                };

                Assert.Equal(1, sale.Id);
                Assert.Equal(10, sale.ProductId);
                Assert.Equal(5, sale.ClientId);
                Assert.Equal(500m, sale.Price);
                Assert.Equal("Card", sale.PaymentType);
                Assert.Equal(2, sale.Quantity);
                Assert.Equal(1, sale.SellerId);
            }

            [Fact]
            public void Client_Model_HasAllProperties()
            {
                var client = new Client
                {
                    Id = 1,
                    FullName = "Ivanov Ivan",
                    Phone = "+123456789",
                    Email = "ivanov@test.com",
                    PurchaseHistory = "History"
                };

                Assert.Equal(1, client.Id);
                Assert.Equal("Ivanov Ivan", client.FullName);
                Assert.Equal("+123456789", client.Phone);
                Assert.Equal("ivanov@test.com", client.Email);
                Assert.Equal("History", client.PurchaseHistory);
            }

            [Fact]
            public void Supplier_Model_HasAllProperties()
            {
                var supplier = new Supplier
                {
                    Id = 1,
                    Name = "Supplier Inc.",
                    ContactPerson = "John Doe",
                    Phone = "123-456",
                    Email = "john@supplier.com",
                    SupplyHistory = "History"
                };

                Assert.Equal(1, supplier.Id);
                Assert.Equal("Supplier Inc.", supplier.Name);
                Assert.Equal("John Doe", supplier.ContactPerson);
                Assert.Equal("123-456", supplier.Phone);
                Assert.Equal("john@supplier.com", supplier.Email);
                Assert.Equal("History", supplier.SupplyHistory);
            }

            [Fact]
            public void Seller_Model_HasAllProperties()
            {
                var seller = new Seller
                {
                    Id = 1,
                    Name = "Seller Name"
                };

                Assert.Equal(1, seller.Id);
                Assert.Equal("Seller Name", seller.Name);
            }

            [Fact]
            public void WarrantyService_Model_HasAllProperties()
            {
                var warranty = new WarrantyService
                {
                    Id = 1,
                    ProductId = 10,
                    ClientId = 5,
                    IssueDescription = "Broken glass",
                    AcceptanceDate = DateTime.Now,
                    CompletionDate = DateTime.Now.AddMonths(1),
                    Status = "Active"
                };

                Assert.Equal(1, warranty.Id);
                Assert.Equal(10, warranty.ProductId);
                Assert.Equal(5, warranty.ClientId);
                Assert.Equal("Broken glass", warranty.IssueDescription);
                Assert.Equal("Active", warranty.Status);
            }

            [Fact]
            public void DiscountProductDto_HasAllProperties()
            {
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

                Assert.Equal(1, dto.Id);
                Assert.Equal("Test Watch", dto.DisplayName);
                Assert.Equal("Rolex", dto.Brand);
                Assert.Equal("Mechanical", dto.Type);
                Assert.True(dto.IsPremiumBrand);
                Assert.Equal(5000m, dto.Price);
                Assert.Equal(10, dto.QuantityInStock);
                Assert.Equal(5, dto.TotalSalesCount);
                Assert.Equal(0.1, dto.AppliedDiscount);
                Assert.Equal(4500m, dto.DiscountedPrice);
            }
        }

        // Тесты для конвертеров
        public class ConverterAdditionalTests
        {
            [Fact]
            public void NullToVisibilityConverter_Convert_Null_ReturnsCollapsed()
            {
                var converter = new NullToVisibilityConverter();
                var result = converter.Convert(null, typeof(object), null, CultureInfo.InvariantCulture);
                Assert.Equal(System.Windows.Visibility.Collapsed, result);
            }

            [Fact]
            public void NullToVisibilityConverter_Convert_NotNull_ReturnsVisible()
            {
                var converter = new NullToVisibilityConverter();
                var result = converter.Convert("test", typeof(object), null, CultureInfo.InvariantCulture);
                Assert.Equal(System.Windows.Visibility.Visible, result);
            }

            [Fact]
            public void CurrentPriceConverter_Convert_WithDiscount_ReturnsDiscountedPrice()
            {
                var converter = new CurrentPriceConverter();
                var product = new Product { Price = 1000m, DiscountedPrice = 800m };
                var result = converter.Convert(product, typeof(string), null, CultureInfo.InvariantCulture);
                Assert.Equal("800.00", result);
            }

            [Fact]
            public void CurrentPriceConverter_Convert_WithoutDiscount_ReturnsOriginalPrice()
            {
                var converter = new CurrentPriceConverter();
                var product = new Product { Price = 1500.50m, DiscountedPrice = null };
                var result = converter.Convert(product, typeof(string), null, CultureInfo.InvariantCulture);
                Assert.Equal("1500.50", result);
            }

            [Fact]
            public void CurrentPriceConverter_Convert_NonProduct_ReturnsZero()
            {
                var converter = new CurrentPriceConverter();
                var result = converter.Convert("not a product", typeof(string), null, CultureInfo.InvariantCulture);
                Assert.Equal("0.00", result);
            }

            [Fact]
            public void ImagePathConverter_Convert_Null_ReturnsNull()
            {
                var converter = new ImagePathConverter();
                var result = converter.Convert(null, typeof(BitmapImage), null, CultureInfo.InvariantCulture);
                Assert.Null(result);
            }

            [Fact]
            public void ImagePathConverter_Convert_Empty_ReturnsNull()
            {
                var converter = new ImagePathConverter();
                var result = converter.Convert(string.Empty, typeof(BitmapImage), null, CultureInfo.InvariantCulture);
                Assert.Null(result);
            }

            [Fact]
            public void ImagePathConverter_Convert_NonExistentFile_ReturnsNull()
            {
                var converter = new ImagePathConverter();
                var result = converter.Convert("non_existent.jpg", typeof(BitmapImage), null,
                    CultureInfo.InvariantCulture);
                Assert.Null(result);
            }
        }

        // Тесты для SimpleDiscountOptimizer
        public class DiscountOptimizerAdditionalTests
        {
            private readonly SimpleDiscountOptimizer _optimizer = new();

            [Theory]
            [InlineData(0, 0.05, 0.30, 0.30)]
            [InlineData(3, 0.05, 0.30, 0.30)]
            [InlineData(6, 0.05, 0.30, 0.20)]
            [InlineData(15, 0.05, 0.30, 0.20)]
            [InlineData(21, 0.05, 0.30, 0.12)]
            [InlineData(50, 0.05, 0.30, 0.12)]
            public void CalculateOptimalDiscount_VariousSalesCounts(int salesCount, double min, double max,
                double expected)
            {
                var product = new DiscountProductDto { TotalSalesCount = salesCount, IsPremiumBrand = false };
                var result = _optimizer.CalculateOptimalDiscount(product, min, max);
                Assert.Equal(expected, result);
            }

            [Fact]
            public void CalculateOptimalDiscount_PremiumBrand_CapsAt15Percent()
            {
                var product = new DiscountProductDto { TotalSalesCount = 3, IsPremiumBrand = true };
                var result = _optimizer.CalculateOptimalDiscount(product, 0.05, 0.30);
                Assert.Equal(0.15, result);
            }

            [Fact]
            public void CalculateOptimalDiscount_CustomMinMax_UsesCorrectRange()
            {
                var product = new DiscountProductDto { TotalSalesCount = 10, IsPremiumBrand = false };
                var result = _optimizer.CalculateOptimalDiscount(product, 0.10, 0.20);
                Assert.Equal(0.16, result);
            }

            [Theory]
            [InlineData(0, 0.0, 3)]
            [InlineData(5, 0.1, 4)]
            [InlineData(10, 0.1, 5)]
            [InlineData(20, 0.2, 7)]
            [InlineData(50, 0.3, 12)]
            public void PredictSales_VariousScenarios(int salesCount, double discount, int expected)
            {
                var product = new DiscountProductDto { TotalSalesCount = salesCount };
                var result = _optimizer.PredictSales(product, discount);
                Assert.Equal(expected, result);
            }
        }
    }
}
