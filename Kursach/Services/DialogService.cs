using System.Windows;
using Kursach.Model.Interfaces;

namespace Kursach.Services
{
    public class DialogService : IDialogService
    {
        public void ShowMessage(string message, string title = "Сообщение", bool isError = false)
        {
            MessageBoxImage icon = isError ? MessageBoxImage.Error : MessageBoxImage.Information;
            MessageBox.Show(message, title, MessageBoxButton.OK, icon);
        }
    }
}