using System.Windows;
using NotepadClone.Views;

namespace NotepadClone.Services
{
    public class DialogService : IDialogService
    {
        public MessageBoxResult Show(string message, string title)
        {
            return MessageBox.Show(message, title, MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
        }

        public string ShowInputDialog(string title, string message)
        {
            var dialog = new InputDialog(title, message)
            {
                Owner = Application.Current.MainWindow
            };

            return dialog.ShowDialog() == true ? dialog.ResponseText : null;
        }
    }
}