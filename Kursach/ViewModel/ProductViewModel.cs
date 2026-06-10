using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kursach.Model.Models;
using Kursach.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kursach.ViewModel;

public partial class ProductViewModel : ObservableObject
{
    private readonly ProductRepository _repository;
    private readonly SupplierRepository _supplierRepository;
    private readonly AppDbContext _context;

    // Режим работы: false = добавление, true = редактирование
    public bool IsEditMode { get; }

    // Id товара при редактировании (null для нового)
    public int? Id { get; }

    // Временное хранение SupplierId для редактирования
    private int? _pendingSupplierId;

    public string FormTitle => IsEditMode ? "Редактирование товара" : "Добавление товара";

    public ProductViewModel()
    {
        _context = new AppDbContext();
        _repository = new ProductRepository(_context);
        _supplierRepository = new SupplierRepository(_context);
        IsEditMode = false;
        LoadSuppliersAsync();
    }

    public ProductViewModel(Product product) : this()
    {
        IsEditMode = true;
        Id = product.Id;

        Type = product.Type;
        Brand = product.Brand;
        Model = product.Model;
        SerialNumber = product.SerialNumber;
        Price = product.Price.ToString("F2");
        QuantityInStock = product.QuantityInStock.ToString();
        WarrantyPeriod = product.WarrantyPeriod.ToString();
        IsPremiumBrand = product.IsPremiumBrand;
        ImagePath = product.ImagePath;
        ImagePreviewPath = product.ImagePath;

        if (product.SupplierId.HasValue)
            _pendingSupplierId = product.SupplierId.Value;
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string _type = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string _brand = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string _model = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string _serialNumber = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string _price = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string _quantityInStock = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string _warrantyPeriod = string.Empty;

    [ObservableProperty]
    private bool _isPremiumBrand;

    [ObservableProperty]
    private string _imagePath = string.Empty;

    [ObservableProperty]
    private string _imagePreviewPath = string.Empty;


    [ObservableProperty]
    private List<Supplier> _suppliers = new();

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private Supplier? _selectedSupplier;

    private async void LoadSuppliersAsync()
    {
        try
        {
            Suppliers = await _supplierRepository.GetAllAsync();

            if (_pendingSupplierId.HasValue)
            {
                SelectedSupplier = Suppliers.FirstOrDefault(s => s.Id == _pendingSupplierId.Value);
            }
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Ошибка при загрузке поставщиков: {ex.Message}", "Ошибка",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private void SelectImage()
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "Image files (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp|All files (*.*)|*.*",
            Title = "Выберите изображение товара"
        };

        if (dialog.ShowDialog() == true)
        {
            ImagePath = dialog.FileName;
            ImagePreviewPath = ImagePath;
        }
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveAsync()
    {
        try
        {
            string savedImagePath = ImagePath;

            // Если выбрали новое локальное изображение – копируем
            if (!string.IsNullOrWhiteSpace(ImagePath) && System.IO.File.Exists(ImagePath))
            {
                var productsImageDir = System.IO.Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Media",
                    "Products");

                if (!System.IO.Directory.Exists(productsImageDir))
                {
                    System.IO.Directory.CreateDirectory(productsImageDir);
                }

                var fileName = $"{Guid.NewGuid()}{System.IO.Path.GetExtension(ImagePath)}";
                var fullPath = System.IO.Path.Combine(productsImageDir, fileName);
                System.IO.File.Copy(ImagePath, fullPath, true);

                // относительный путь
                savedImagePath = System.IO.Path.Combine("Media", "Products", fileName);
            }

            if (!IsEditMode)
            {
                // Добавление нового товара
                var product = new Product
                {
                    Type = Type,
                    Brand = Brand,
                    Model = Model,
                    SerialNumber = SerialNumber,
                    Price = decimal.Parse(Price),
                    QuantityInStock = int.Parse(QuantityInStock),
                    WarrantyPeriod = int.Parse(WarrantyPeriod),
                    ImagePath = savedImagePath,
                    SupplierId = SelectedSupplier?.Id,
                    IsPremiumBrand = IsPremiumBrand
                };

                await _repository.AddAsync(product);

                // Очистка формы
                Type = string.Empty;
                Brand = string.Empty;
                Model = string.Empty;
                SerialNumber = string.Empty;
                Price = string.Empty;
                QuantityInStock = string.Empty;
                WarrantyPeriod = string.Empty;
                ImagePath = string.Empty;
                ImagePreviewPath = string.Empty;
                SelectedSupplier = null;
                IsPremiumBrand = false;

                System.Windows.MessageBox.Show("Товар успешно добавлен!", "Успех",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
            else
            {
                // Редактирование существующего товара
                if (Id == null)
                    return;

                var product = await _repository.GetByIdAsync(Id.Value);
                if (product == null)
                    return;

                product.Type = Type;
                product.Brand = Brand;
                product.Model = Model;
                product.SerialNumber = SerialNumber;
                product.Price = decimal.Parse(Price);
                product.QuantityInStock = int.Parse(QuantityInStock);
                product.WarrantyPeriod = int.Parse(WarrantyPeriod);
                product.IsPremiumBrand = IsPremiumBrand;

                if (!string.IsNullOrWhiteSpace(savedImagePath))
                    product.ImagePath = savedImagePath;

                product.SupplierId = SelectedSupplier?.Id;

                await _repository.UpdateAsync(product);

                System.Windows.MessageBox.Show("Изменения успешно сохранены!", "Успех",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    private bool CanSave()
    {
        return !string.IsNullOrWhiteSpace(Type) &&
               !string.IsNullOrWhiteSpace(Brand) &&
               !string.IsNullOrWhiteSpace(Model) &&
               !string.IsNullOrWhiteSpace(SerialNumber) &&
               decimal.TryParse(Price, out _) &&
               int.TryParse(QuantityInStock, out _) &&
               int.TryParse(WarrantyPeriod, out _) &&
               SelectedSupplier != null;
    }

}
