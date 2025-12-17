using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kursach.Model.Models;
using Kursach.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kursach.ViewModel;
namespace Kursach.ViewModel;

public partial class MainViewModel : ObservableObject
{
    private readonly ProductRepository _productRepository;
    private readonly AppDbContext _context;

    [ObservableProperty]
    private List<Product> _products = new();

    [ObservableProperty]
    private List<Product> _filteredProducts = new();

    [ObservableProperty]
    private List<string> _types = new();

    [ObservableProperty]
    private List<string> _brands = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FilteredProducts))]
    private string? _selectedType;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FilteredProducts))]
    private string? _selectedBrand;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FilteredProducts))]
    private string _minPrice = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FilteredProducts))]
    private string _maxPrice = string.Empty;

    public MainViewModel()
    {
        _context = new AppDbContext();
        _productRepository = new ProductRepository(_context);
        LoadProductsAsync();
    }

    private async void LoadProductsAsync()
    {
        try
        {
            Products = await _productRepository.GetAllAsync();
            UpdateFilterLists();
            
            if (Types.Count > 0 && SelectedType == null)
                SelectedType = "Все";
            if (Brands.Count > 0 && SelectedBrand == null)
                SelectedBrand = "Все";
                
            ApplyFilters();
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Ошибка при загрузке товаров: {ex.Message}", "Ошибка",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    partial void OnProductsChanged(List<Product> value)
    {
        UpdateFilterLists();
        ApplyFilters();
    }

    partial void OnSelectedTypeChanged(string? value)
    {
        ApplyFilters();
    }



    partial void OnSelectedBrandChanged(string? value)
    {
        ApplyFilters();
    }

    partial void OnMinPriceChanged(string value)
    {
        ApplyFilters();
    }

    partial void OnMaxPriceChanged(string value)
    {
        ApplyFilters();
    }

    private void UpdateFilterLists()
    {
        var types = Products.Select(p => p.Type)
            .Distinct()
            .OrderBy(t => t)
            .ToList();
        types.Insert(0, "Все");
        Types = types;

        var brands = Products.Select(p => p.Brand)
            .Distinct()
            .OrderBy(b => b)
            .ToList();
        brands.Insert(0, "Все");
        Brands = brands;
    }

    private void ApplyFilters()
    {
        var filtered = Products.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SelectedType) && SelectedType != "Все")
        {
            filtered = filtered.Where(p => p.Type == SelectedType);
        }

        if (!string.IsNullOrWhiteSpace(SelectedBrand) && SelectedBrand != "Все")
        {
            filtered = filtered.Where(p => p.Brand == SelectedBrand);
        }

        if (decimal.TryParse(MinPrice, out var minPriceValue))
        {
            filtered = filtered.Where(p => p.Price >= minPriceValue);
        }

        if (decimal.TryParse(MaxPrice, out var maxPriceValue))
        {
            filtered = filtered.Where(p => p.Price <= maxPriceValue);
        }

        FilteredProducts = filtered.ToList();
    }

    [RelayCommand]
    private void ClearFilters()
    {
        SelectedType = "Все";
        SelectedBrand = "Все";
        MinPrice = string.Empty;
        MaxPrice = string.Empty;
    }

    [RelayCommand]
    private void CreateSale(Product? product)
    {
        if (product == null) return;
        
        var mainWindow = App.Current.MainWindow as MainWindow;
        if (mainWindow != null)
        {
            var tabControl = mainWindow.FindName("MainTabControl") as System.Windows.Controls.TabControl;
            if (tabControl == null)
            {
                tabControl = FindVisualChild<System.Windows.Controls.TabControl>(mainWindow);
            }
            
            if (tabControl != null)
            {
                tabControl.SelectedIndex = 1;
                
                var formView = FindVisualChild<Kursach.View.FormView>(mainWindow);
                if (formView?.DataContext is FormsViewModel fvm)
                {
                    var sellViewModel = new SellViewModel(product);
                    var sellView = new Kursach.View.Forms.NewSell();
                    sellView.DataContext = sellViewModel;
                    fvm.CurrentView = sellView;
                }
            }
        }
    }
    
    private static T? FindVisualChild<T>(System.Windows.DependencyObject parent) where T : System.Windows.DependencyObject
    {
        for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);
            if (child is T result)
                return result;
            
            var childOfChild = FindVisualChild<T>(child);
            if (childOfChild != null)
                return childOfChild;
        }
        return null;
    }

    public void RefreshProducts()
    {
        LoadProductsAsync();
    }
    
    [RelayCommand]
    private async Task DeleteProduct(Product? product)
    {
        if (product == null) return;

        var result = System.Windows.MessageBox.Show(
            $"Удалить товар {product.Brand} {product.Model}?",
            "Подтверждение",
            System.Windows.MessageBoxButton.YesNo,
            System.Windows.MessageBoxImage.Question);

        if (result != System.Windows.MessageBoxResult.Yes)
            return;

        try
        {
            await _productRepository.DeleteAsync(product.Id);
            RefreshProducts();
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(
                $"Ошибка при удалении товара: {ex.Message}",
                "Ошибка",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Error);
        }
    }

    
    
    [RelayCommand]
    private void EditProduct(Product? product)
    {
        if (product == null) return;

        var mainWindow = App.Current.MainWindow as MainWindow;
        if (mainWindow == null) return;

        var tabControl = mainWindow.FindName("MainTabControl") as System.Windows.Controls.TabControl;
        if (tabControl == null)
        {
            tabControl = FindVisualChild<System.Windows.Controls.TabControl>(mainWindow);
        }

        if (tabControl != null)
        {
            // переключаемся на вкладку с формами (как в CreateSale)
            tabControl.SelectedIndex = 1;

            var formView = FindVisualChild<Kursach.View.FormView>(mainWindow);
            if (formView?.DataContext is FormsViewModel fvm)
            {
                // создаем ViewModel для формы редактирования товара
                var productViewModel = new ProductViewModel(product); // конструктор с Product
                var productView = new Kursach.View.Forms.NewProduct
                {
                    DataContext = productViewModel
                };

                fvm.CurrentView = productView;
            }
        }
    }


    
    
}