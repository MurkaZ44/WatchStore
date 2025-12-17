using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.Input;
using Kursach.View.Forms;

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
        // Initialize _currentView to a default view to prevent CS8618 warning
        _currentView = new NewProduct() { DataContext = new ProductViewModel() };
        FormCommand = new RelayCommand<string?>(Navigate);
    }
    private void Navigate(string? destination)
    {
        switch (destination)
        {
            case "Product":
                var productView = new NewProduct();
                productView.DataContext = new ProductViewModel();
                CurrentView = productView;
                break;
            case "Sell":
                var sellView = new NewSell();
                sellView.DataContext = new SellViewModel();
                CurrentView = sellView;
                break;
            case "Warranty":
                var warrantyView = new NewWarranty();
                warrantyView.DataContext = new WarrantyViewModel();
                CurrentView = warrantyView;
                break;
            case "Seller":
                var sellerView = new NewSeller();
                sellerView.DataContext = new SellerViewModel();
                CurrentView = sellerView;
                break;
            case "Supplier":
                var supplierView = new NewSupplier();
                supplierView.DataContext = new SupplierViewModel();
                CurrentView = supplierView;
                break;
            case "Client":
                var clientView = new NewClient();
                clientView.DataContext = new ClientViewModel();
                CurrentView = clientView;
                break;
        }
    }
    
    public event PropertyChangedEventHandler? PropertyChanged; // Fixed CS8612
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) // Fixed CS8625
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}