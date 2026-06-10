using Xunit;
using Kursach.ViewModel;
using Kursach.Model.Interfaces;
using Kursach.Model.Models;

namespace TestProject1
{
    // Тесты для MainViewModel
    public class MainViewModelTests
    {
        [Fact]
        public void MainViewModel_CanBeInitialized()
        {
            // Arrange & Act
            var vm = new MainViewModel();

            // Assert
            Assert.NotNull(vm);
        }
    }

    // Тесты для ProductViewModel
    public class ProductViewModelTests
    {
        [Fact]
        public void ProductViewModel_CanBeInitialized()
        {
            // Arrange & Act
            var vm = new ProductViewModel();

            // Assert
            Assert.NotNull(vm);
            Assert.NotNull(vm.Suppliers);
        }
    }

    // Тесты для ClientListViewModel
    public class ClientListViewModelTests
    {
        [Fact]
        public void ClientListViewModel_CanBeInitialized()
        {
            // Arrange & Act
            var vm = new ClientListViewModel();

            // Assert
            Assert.NotNull(vm);
            Assert.NotNull(vm.Clients);
        }
    }
}