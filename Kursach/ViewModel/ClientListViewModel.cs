using CommunityToolkit.Mvvm.ComponentModel;
using Kursach.Model.Models;
using Kursach.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace Kursach.ViewModel;

public partial class ClientListViewModel : ObservableObject
{
    private readonly ClientRepository _clientRepository;

    [ObservableProperty]
    private ObservableCollection<Client> _clients;

    public ClientListViewModel()
    {
        var context = new AppDbContext();
        _clientRepository = new ClientRepository(context);
        _clients = new ObservableCollection<Client>();
        LoadClientsAsync();
    }

    private async Task LoadClientsAsync()
    {
        try
        {
            var clientsList = await _clientRepository.GetAllAsync();
            Clients = new ObservableCollection<Client>(clientsList);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при загрузке клиентов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}