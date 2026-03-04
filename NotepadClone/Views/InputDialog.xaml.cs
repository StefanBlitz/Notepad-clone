using System.Windows;

namespace NotepadClone.Views
{
    public partial class InputDialog : Window
    {
        public string ResponseText => InputBox.Text;

        public InputDialog(string title, string message)
        {
            InitializeComponent();
            Title = title;
            MessageText.Text = message;
            InputBox.Focus();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}