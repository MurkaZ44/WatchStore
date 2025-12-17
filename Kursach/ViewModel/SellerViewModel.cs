using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kursach.Model.Models;
using Kursach.Model.Repositories;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace Kursach.ViewModel;

public partial class SellerViewModel : ObservableObject
{
    private readonly SellerRepository _sellerRepository;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveSellerCommand))]
    private string _sellerName = string.Empty;

    public SellerViewModel()
    {
        var context = new AppDbContext();
        _sellerRepository = new SellerRepository(context);
    }

    [RelayCommand(CanExecute = nameof(CanSaveSeller))]
    private async Task SaveSellerAsync()
    {
        try
        {
            var newSeller = new Seller { Name = SellerName };
            await _sellerRepository.AddAsync(newSeller);
            MessageBox.Show("Продавец успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            SellerName = string.Empty; // Очистка поля после сохранения
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при добавлении продавца: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private bool CanSaveSeller()
    {
        return !string.IsNullOrWhiteSpace(SellerName);
    }
}