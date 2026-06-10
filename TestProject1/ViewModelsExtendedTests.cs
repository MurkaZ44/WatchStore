using Xunit;
using Kursach.ViewModel;
using Kursach.Model.Interfaces;
using Kursach.Model.Models;
using System.Collections.ObjectModel;
using Kursach.View;

namespace TestProject1
{
    // Тесты для SellViewModel
    public class SellViewModelTests
    {
        [Fact]
        public void SellViewModel_CanBeInitialized()
        {
            // Arrange & Act
            var vm = new SellViewModel();

            // Assert
            Assert.NotNull(vm);
            Assert.NotNull(vm.Clients);
            Assert.NotNull(vm.Sellers);
        }
    }

    // Тесты для SupplierViewModel
    public class SupplierViewModelTests
    {
        [Fact]
        public void SupplierViewModel_CanBeInitialized()
        {
            // Arrange & Act
            var vm = new SupplierViewModel();

            // Assert
            Assert.NotNull(vm);
        }
    }

    // Тесты для SellerViewModel
    public class SellerViewModelTests
    {
        [Fact]
        public void SellerViewModel_CanBeInitialized()
        {
            // Arrange & Act
            var vm = new SellerViewModel();

            // Assert
            Assert.NotNull(vm);
        }
    }

    // Тесты для ReportsViewModel
    public class ReportsViewModelTests
    {
        [Fact]
        public void ReportsViewModel_CanBeInitialized()
        {
            // Arrange & Act
            var dialog = new MockDialogService();
            var vm = new ReportsViewModel(dialog);

            // Assert
            Assert.NotNull(vm);
        }
    }

    // Тесты для DiscountsViewModel
    public class DiscountsViewModelTests
    {
        [Fact]
        public void DiscountsViewModel_CanBeInitialized()
        {
            // Arrange & Act
            var vm = new DiscountsViewModel();

            // Assert
            Assert.NotNull(vm);
            Assert.NotNull(vm.AvailableProducts);
        }
    }
}