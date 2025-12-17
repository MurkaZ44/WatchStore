using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kursach.Model.Models;
using Kursach.Model.Repositories;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace Kursach.ViewModel;

public partial class SupplierViewModel : ObservableObject
{
    private readonly SupplierRepository _supplierRepository;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveSupplierCommand))]
    private string _supplierName = string.Empty;
    
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveSupplierCommand))]
    private string _contactPerson = string.Empty;
    
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveSupplierCommand))]
    private string _phone = string.Empty;
    
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveSupplierCommand))]
    private string _email = string.Empty;

    public SupplierViewModel()
    {
        var context = new AppDbContext();
        _supplierRepository = new SupplierRepository(context);
    }

    [RelayCommand(CanExecute = nameof(CanSaveSupplier))]
    private async Task SaveSupplierAsync()
    {
        try
        {
            var newSupplier = new Supplier 
            { 
                Name = SupplierName,
                ContactPerson = ContactPerson,
                Phone = Phone,
                Email = Email
            };
            await _supplierRepository.AddAsync(newSupplier);
            MessageBox.Show("Поставщик успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            
            // Очистка полей
            SupplierName = string.Empty;
            ContactPerson = string.Empty;
            Phone = string.Empty;
            Email = string.Empty;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при добавлении поставщика: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private bool CanSaveSupplier()
    {
        return !string.IsNullOrWhiteSpace(SupplierName) && 
               !string.IsNullOrWhiteSpace(ContactPerson) && 
               !string.IsNullOrWhiteSpace(Phone) && 
               !string.IsNullOrWhiteSpace(Email);
    }
}