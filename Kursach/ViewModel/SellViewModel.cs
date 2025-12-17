using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kursach.Model.Models;
using Kursach.Model.Repositories;

namespace Kursach.ViewModel;

public partial class SellViewModel : ObservableObject
{
    private readonly SaleRepository _saleRepository;
    private readonly ClientRepository _clientRepository;
    private readonly ProductRepository _productRepository;
    private readonly SellerRepository _sellerRepository;
    private readonly AppDbContext _context;
    private readonly Product? _selectedProduct;

    [ObservableProperty]
    private List<Client> _clients = new();

    [ObservableProperty]
    private List<Seller> _sellers = new();

    [ObservableProperty]
    private Client? _selectedClient;

    [ObservableProperty]
    private Seller? _selectedSeller;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private DateTime? _saleDate;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string _price = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string _paymentType = string.Empty;

    // [ObservableProperty]
    // [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    // private string _seller = string.Empty; // Заменено на SelectedSeller

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private int _quantity = 1;

    public string SelectedProductDisplay { get; }

    public List<string> PaymentTypes { get; } = new() { "Наличные", "Карта", "Банковский перевод", "Онлайн оплата" };

    public SellViewModel(Product? product = null)
    {
        _context = new AppDbContext();
        _saleRepository = new SaleRepository(_context);
        _clientRepository = new ClientRepository(_context);
        _productRepository = new ProductRepository(_context);
        _sellerRepository = new SellerRepository(_context); // <--- ДОБАВЛЕНО
        _selectedProduct = product;

        if (product != null)
        {
            SelectedProductDisplay = $"{product.Brand} {product.Model} - {product.Price:F2} ₽";
            Price = product.Price.ToString("F2");
        }
        else
        {
            SelectedProductDisplay = "Товар не выбран";
        }

        SaleDate = DateTime.Now;
        LoadClientsAndSellersAsync();
    }

    private async void LoadClientsAndSellersAsync()
    {
        try
        {
            Clients = await _clientRepository.GetAllAsync();
            Sellers = await _sellerRepository.GetAllAsync();
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveAsync()
    {
        if (_selectedProduct == null)
        {
            System.Windows.MessageBox.Show("Товар не выбран!", "Ошибка",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }

        try
        {
            if (!decimal.TryParse(Price, out var priceValue))
            {
                System.Windows.MessageBox.Show("Неверный формат цены!", "Ошибка",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }

            if (SelectedClient == null)
            {
                System.Windows.MessageBox.Show("Выберите клиента!", "Ошибка",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return;
            }

            if (SaleDate == null)
            {
                System.Windows.MessageBox.Show("Выберите дату продажи!", "Ошибка",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return;
            }

            if (Quantity > _selectedProduct.QuantityInStock)
            {
                System.Windows.MessageBox.Show($"Недостаточно товара на складе! Доступно: {_selectedProduct.QuantityInStock}", "Ошибка",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }

            var sale = new Sale
            {
                ProductId = _selectedProduct.Id,
                ClientId = SelectedClient.Id,
                Date = SaleDate.Value,
                Price = priceValue,
                Quantity = this.Quantity,
                PaymentType = PaymentType,
                SellerId = SelectedSeller.Id // <--- ИЗМЕНЕНО
            };

            await _saleRepository.AddAsync(sale);

            // Уменьшаем количество товара на складе
            _selectedProduct.QuantityInStock -= this.Quantity; // <--- ИСПРАВЛЕНО
            await _productRepository.UpdateAsync(_selectedProduct);

            System.Windows.MessageBox.Show("Продажа успешно оформлена!", "Успех",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);

            // Очистка формы
            SelectedClient = null;
            SaleDate = DateTime.Now;
            Price = _selectedProduct.Price.ToString("F2");
            PaymentType = string.Empty;
            SelectedSeller = null;
            Quantity = 1;
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    private bool CanSave()
    {
        return _selectedProduct != null &&
               SelectedClient != null &&
               SaleDate != null &&
               !string.IsNullOrWhiteSpace(Price) &&
               decimal.TryParse(Price, out _) &&
               !string.IsNullOrWhiteSpace(PaymentType) &&
               SelectedSeller != null && // <--- ИЗМЕНЕНО
               Quantity > 0;
    }
}