using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.Input;

namespace Kursach.ViewModel;

public class FormsViewModel : INotifyPropertyChanged
{
    private object _currentView;
    public object CurrentView
    {
        get => _currentView;
        set { _currentView = value; OnPropertyChanged(); }
    }
    public ICommand FormCommand { get; }
    public FormsViewModel()
    {
        FormCommand = new RelayCommand<string>(Navigate);
    }
    private void Navigate(string destination)
    {
        switch (destination)
        {
            case "Product":
                CurrentView = new ProductViewModel(); // Создаём VM для товаров
                break;
            case "Client":
                CurrentView = new ClientViewModel(); // VM для клиентов
                break;
            case "Sell":
                CurrentView = new SellViewModel(); // VM для продажи
                break;
            case "Warranty":
                CurrentView = new WarrantyViewModel(); // VM для гарантии
                break;
        }
    }
    
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    
    
    
    
}