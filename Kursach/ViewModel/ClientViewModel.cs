using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kursach.Model.Models;
using Kursach.Model.Repositories;

namespace Kursach.ViewModel;

public partial class ClientViewModel : ObservableObject
{
    private readonly ClientRepository _repository;
    private readonly AppDbContext _context;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string _fullName = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string _phone = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _purchaseHistory = string.Empty;

    public ClientViewModel()
    {
        _context = new AppDbContext();
        _repository = new ClientRepository(_context);
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveAsync()
    {
        try
        {
            var client = new Client
            {
                FullName = FullName,
                Phone = Phone,
                Email = Email,
                PurchaseHistory = PurchaseHistory
            };

            await _repository.AddAsync(client);
            
            // Очистка формы
            FullName = string.Empty;
            Phone = string.Empty;
            Email = string.Empty;
            PurchaseHistory = string.Empty;
            
            System.Windows.MessageBox.Show("Клиент успешно добавлен!", "Успех", 
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", 
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    private bool CanSave()
    {
        return !string.IsNullOrWhiteSpace(FullName) &&
               !string.IsNullOrWhiteSpace(Phone) &&
               !string.IsNullOrWhiteSpace(Email);
    }
}