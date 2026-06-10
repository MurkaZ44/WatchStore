using Xunit;
using Kursach;
using Kursach.Model.Models;
using Kursach.Model.Repositories;
using Microsoft.EntityFrameworkCore;

namespace TestProject1
{
    // Тесты для репозиториев с использованием InMemoryDatabase
    public class RepositoryTests
    {
        private AppDbContext GetInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task ProductRepository_CanAddAndRetrieveProduct()
        {
            // Arrange
            using (var context = GetInMemoryContext("TestDb_Product"))
            {
                var repo = new ProductRepository(context);
                var product = new Product
                {
                    Id = 1,
                    Brand = "Casio",
                    Model = "TestModel",
                    Price = 100m,
                    QuantityInStock = 10
                };

                // Act
                await repo.AddAsync(product);
                var result = await repo.GetByIdAsync(1);

                // Assert
                Assert.NotNull(result);
                Assert.Equal("Casio", result.Brand);
            }
        }

        [Fact]
        public async Task ClientRepository_CanAddAndRetrieveClient()
        {
            // Arrange
            using (var context = GetInMemoryContext("TestDb_Client"))
            {
                var repo = new ClientRepository(context);
                var client = new Client
                {
                    Id = 1,
                    FullName = "Test Client",
                    Phone = "12345"
                };

                // Act
                await repo.AddAsync(client);
                var result = await repo.GetByIdAsync(1);

                // Assert
                Assert.NotNull(result);
                Assert.Equal("Test Client", result.FullName);
            }
        }

        [Fact]
        public async Task SaleRepository_CanAddAndRetrieveSale()
        {
            // Arrange
            using (var context = GetInMemoryContext("TestDb_Sale"))
            {
                // Сначала добавим продукт и клиента, так как Sale имеет внешние ключи
                var product = new Product { Id = 1, Brand = "B", Model = "M", Price = 10m, QuantityInStock = 1 };
                var client = new Client { Id = 1, FullName = "C", Phone = "P" };
                context.Products.Add(product);
                context.Clients.Add(client);
                context.SaveChanges();

                var repo = new SaleRepository(context);
                var sale = new Sale
                {
                    Id = 1,
                    ProductId = 1,
                    ClientId = 1,
                    Price = 10m,
                    Quantity = 1,
                    Date = System.DateTime.Now
                };

                // Act
                await repo.AddAsync(sale);
                var result = await repo.GetByIdAsync(1);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(10m, result.Price);
            }
        }
    }
}