using System.Windows;
using System.Windows.Media;
using Kursach.Model.Repositories;
using Kursach.ViewModel;

namespace Kursach.View;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
public partial class DiscountsViewModel : ObservableObject
{
    private readonly ProductRepository _productRepository;
    private readonly AppDbContext _context;
    private readonly IDiscountOptimizer _discountOptimizer;
    private double? _lastCalculatedDiscount;

    [ObservableProperty]
    private ObservableCollection<DiscountProductDto> _availableProducts = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectedProductDescription))]
    private DiscountProductDto? _selectedProduct;

    [ObservableProperty]
    private double _minDiscount = 0.05; // 5 %

    [ObservableProperty]
    private double _maxDiscount = 0.30; // 30 %

    [ObservableProperty]
    private string _calculatedDiscountText = "Скидка не рассчитана";

    [ObservableProperty]
    private string _expectedProfitText = string.Empty;

    public string SelectedProductDescription =>
        SelectedProduct == null
            ? "Выберите товар для расчета скидки"
            : $"{SelectedProduct.Brand} {SelectedProduct.DisplayName} ({SelectedProduct.Type}), " +
              $"цена: {SelectedProduct.Price:F2} ₽, на складе: {SelectedProduct.QuantityInStock} шт.";

    public DiscountsViewModel()
    {
        _context = new AppDbContext();
        _productRepository = new ProductRepository(_context);
        _discountOptimizer = new SimpleDiscountOptimizer(); // сюда потом можно подставить регрессию из курсовой

        LoadProductsAsync();
    }

    private async void LoadProductsAsync()
    {
        try
        {
            var products = await _productRepository.GetAllAsync();

            var list = products
                .Where(p => p.QuantityInStock > 0)
                .Select(p => new DiscountProductDto
                {
                    Id = p.Id,
                    DisplayName = $"{p.Model} ({p.SerialNumber})",
                    Brand = p.Brand,
                    Type = p.Type,
                    IsPremiumBrand = p.IsPremiumBrand,
                    Price = p.Price,
                    QuantityInStock = p.QuantityInStock,
                    TotalSalesCount = p.Sales?.Sum(s => s.Quantity) ?? 0
                })
                .OrderBy(p => p.Brand)
                .ThenBy(p => p.DisplayName);

            AvailableProducts = new ObservableCollection<DiscountProductDto>(list);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Ошибка при загрузке товаров для скидок: {ex.Message}",
                "Ошибка", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private void CalculateDiscount()
    {
        // 1. Проверяем, что выбран товар
        if (SelectedProduct == null)
            return;

        // 2. Нормализуем диапазон скидки
        if (MinDiscount < 0) MinDiscount = 0;
        if (MaxDiscount > 0.9) MaxDiscount = 0.9;
        if (MaxDiscount <= MinDiscount)
            MaxDiscount = MinDiscount + 0.05;

        // 3. Вызываем оптимизатор (регрессионная модель внутри IDiscountOptimizer)
        double optimalDiscount = _discountOptimizer.CalculateOptimalDiscount(
            SelectedProduct,
            MinDiscount,
            MaxDiscount);

        _lastCalculatedDiscount = optimalDiscount;

        // 4. Прогнозируем продажи при этой скидке
        int predictedSales = _discountOptimizer.PredictSales(SelectedProduct, optimalDiscount);

        // 5. Считаем цену после скидки и выручку
        double priceAfterDiscount = (double)SelectedProduct.Price * (1 - optimalDiscount);
        double revenue = priceAfterDiscount * predictedSales;

        // 6. Обновляем текстовые свойства для привязки в XAML
        CalculatedDiscountText = $"Рекомендуемая скидка: {optimalDiscount:P0}";
        ExpectedProfitText = $"Прогноз продаж: {predictedSales} шт., выручка ~ {revenue:F2} ₽";
    }

    [RelayCommand]
    private async Task CancelDiscount()
    {
        if (SelectedProduct == null)
            return;

        // 1. Set DiscountedPrice and AppliedDiscount of SelectedProduct to null
        SelectedProduct.DiscountedPrice = null;
        SelectedProduct.AppliedDiscount = null;

        // 2. Retrieve the corresponding Product model from the database
        var product = await _productRepository.GetByIdAsync(SelectedProduct.Id);
        if (product == null)
        {
            System.Windows.MessageBox.Show("Товар не найден в базе данных.", "Ошибка", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            return;
        }

        // 3. Update DiscountedPrice and AppliedDiscount of the retrieved Product model to null
        product.DiscountedPrice = null;
        product.AppliedDiscount = null;

        // 4. Persist these changes to the database
        await _productRepository.UpdateAsync(product);

        // 5. Call OnPropertyChanged() for DiscountedPrice and AppliedDiscount on the SelectedProduct
        OnPropertyChanged(nameof(SelectedProduct)); // This will notify for all properties of SelectedProduct

        // 6. Refresh the list of products
        LoadProductsAsync();
        
        System.Windows.MessageBox.Show($"Скидка для {SelectedProduct.DisplayName} отменена.", "Отмена скидки", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private bool CanCancelDiscount() => SelectedProduct != null && (SelectedProduct.AppliedDiscount != null || SelectedProduct.DiscountedPrice != null);

    private bool CanCalculateDiscount() => SelectedProduct != null;
    
    [RelayCommand]
    private void ApplyDiscount()
    {
        if (SelectedProduct == null || _lastCalculatedDiscount == null)
            return;

        var product = _context.Products.FirstOrDefault(p => p.Id == SelectedProduct.Id);
        if (product == null)
            return;

        Debug.WriteLine($"[DEBUG] ApplyDiscount: Before DB update - Product ID: {product.Id}, Current AppliedDiscount: {product.AppliedDiscount}, Current DiscountedPrice: {product.DiscountedPrice}");
        Debug.WriteLine($"[DEBUG] ApplyDiscount: _lastCalcululatedDiscount: {_lastCalculatedDiscount.Value}");

        product.AppliedDiscount = _lastCalculatedDiscount;
        product.DiscountedPrice = product.Price * (decimal)(1 - _lastCalculatedDiscount.Value);
        _context.SaveChanges();
        
        Debug.WriteLine($"[DEBUG] ApplyDiscount: After DB update - Product ID: {product.Id}, New AppliedDiscount: {product.AppliedDiscount}, New DiscountedPrice: {product.DiscountedPrice}");

        // Найти MainViewModel и обновить товары
        var mainWindow = App.Current.MainWindow as MainWindow;
        if (mainWindow != null)
        {
            var mainView = FindVisualChild<Kursach.View.MainView>(mainWindow);
            if (mainView?.DataContext is MainViewModel mvm)
            {
                mvm.RefreshProducts();
            }
        }

        ExpectedProfitText += "\nСкидка применена, каталог обновлён.";
        
        // Update the SelectedProduct DTO with the applied discount
        if (SelectedProduct != null)
        {
            Debug.WriteLine($"[DEBUG] ApplyDiscount: Before DTO update - SelectedProduct ID: {SelectedProduct.Id}, Current AppliedDiscount: {SelectedProduct.AppliedDiscount}, Current DiscountedPrice: {SelectedProduct.DiscountedPrice}");

            SelectedProduct.AppliedDiscount = _lastCalculatedDiscount;
            SelectedProduct.DiscountedPrice = product.DiscountedPrice;
            OnPropertyChanged(nameof(SelectedProduct)); // Notify UI of change
            
            Debug.WriteLine($"[DEBUG] ApplyDiscount: After DTO update - SelectedProduct ID: {SelectedProduct.Id}, New AppliedDiscount: {SelectedProduct.AppliedDiscount}, New DiscountedPrice: {SelectedProduct.DiscountedPrice}");
        }
    }

    private static T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T result)
                return result;
            var childOfChild = FindVisualChild<T>(child);
            if (childOfChild != null)
                return childOfChild;
        }
        return null;
    }
}