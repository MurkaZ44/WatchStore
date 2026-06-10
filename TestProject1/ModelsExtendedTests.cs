using Xunit;
using Kursach.Model.Models;

namespace TestProject1
{
    // Тесты для модели Seller
    public class SellerTests
    {
        [Fact]
        public void Seller_CanBeInitialized()
        {
            // Arrange & Act
            var seller = new Seller
            {
                Id = 1,
                Name = "Petr Petrov"
            };

            // Assert
            Assert.Equal(1, seller.Id);
            Assert.Equal("Petr Petrov", seller.Name);
        }
    }

    // Тесты для модели Supplier
    public class SupplierTests
    {
        [Fact]
        public void Supplier_CanBeInitialized()
        {
            // Arrange & Act
            var supplier = new Supplier
            {
                Id = 1,
                Name = "Supplier Inc.",
                ContactPerson = "John Doe",
                Phone = "987-654",
                Email = "john@supplier.com"
            };

            // Assert
            Assert.Equal(1, supplier.Id);
            Assert.Equal("Supplier Inc.", supplier.Name);
            Assert.Equal("John Doe", supplier.ContactPerson);
        }
    }

    // Тесты для модели WarrantyService
    public class WarrantyServiceTests
    {
        [Fact]
        public void WarrantyService_CanBeInitialized()
        {
            // Arrange & Act
            var service = new WarrantyService
            {
                Id = 1,
                ProductId = 10,
                ClientId = 5,
                IssueDescription = "Broken glass",
                AcceptanceDate = System.DateTime.Now,
                CompletionDate = System.DateTime.Now.AddMonths(1),
                Status = "Active"
            };

            // Assert
            Assert.Equal(1, service.Id);
            Assert.Equal("Active", service.Status);
            Assert.Equal("Broken glass", service.IssueDescription);
        }
    }
}