namespace Kursach.Model.Interfaces
{
    public interface IDialogService
    {
        void ShowMessage(string message, string title = "Сообщение", bool isError = false);
    }
}