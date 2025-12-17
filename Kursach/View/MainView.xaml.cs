using System.Windows.Controls;

namespace Kursach.View;

public partial class MainView : UserControl    
{
    public MainView()
    {
        InitializeComponent();
        this.Loaded += MainView_Loaded;
    }

    private void MainView_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        // Обновляем список товаров при загрузке
        if (DataContext is ViewModel.MainViewModel viewModel)
        {
            viewModel.RefreshProducts();
        }
    }
}