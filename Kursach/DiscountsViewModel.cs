using System.Windows;
using System.Windows.Media;
using Kursach.Model.Repositories;
using Kursach.ViewModel;

namespace Kursach.View;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
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

    private bool CanCalculateDiscount() => SelectedProduct != null;
    
    [RelayCommand]
    private void ApplyDiscount()
    {
        if (SelectedProduct == null || _lastCalculatedDiscount == null)
            return;

        var product = _context.Products.FirstOrDefault(p => p.Id == SelectedProduct.Id);
        if (product == null)
            return;

        product.AppliedDiscount = _lastCalculatedDiscount;
        product.DiscountedPrice = product.Price * (decimal)(1 - _lastCalculatedDiscount.Value);
        _context.SaveChanges();

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