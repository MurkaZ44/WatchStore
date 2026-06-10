using Kursach.Model.Interfaces;

namespace TestProject1
{
    public class MockDialogService : IDialogService
    {
        public bool WasCalled { get; private set; }
        public void ShowMessage(string message, string title = "Сообщение", bool isError = false)
        {
            WasCalled = true;
        }
        public bool ShowConfirmation(string message) => true;
        public string? ShowInputDialog(string prompt) => "Test Input";
    }
}