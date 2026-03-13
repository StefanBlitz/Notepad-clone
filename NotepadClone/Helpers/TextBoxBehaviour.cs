using System.Windows;
using System.Windows.Controls;

namespace NotepadClone.Helpers
{
    public static class TextBoxBehavior
    {
        public static readonly DependencyProperty BoundSelectionStartProperty =
            DependencyProperty.RegisterAttached("BoundSelectionStart", typeof(int), typeof(TextBoxBehavior), new PropertyMetadata(0, OnSelectionStartChanged));

        public static readonly DependencyProperty BoundSelectionLengthProperty =
            DependencyProperty.RegisterAttached("BoundSelectionLength", typeof(int), typeof(TextBoxBehavior), new PropertyMetadata(0, OnSelectionLengthChanged));

        public static int GetBoundSelectionStart(DependencyObject obj) => (int)obj.GetValue(BoundSelectionStartProperty);
        public static void SetBoundSelectionStart(DependencyObject obj, int value) => obj.SetValue(BoundSelectionStartProperty, value);

        public static int GetBoundSelectionLength(DependencyObject obj) => (int)obj.GetValue(BoundSelectionLengthProperty);
        public static void SetBoundSelectionLength(DependencyObject obj, int value) => obj.SetValue(BoundSelectionLengthProperty, value);

        private static void OnSelectionStartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                textBox.SelectionStart = (int)e.NewValue;
                textBox.Focus(); 
            }
        }

        private static void OnSelectionLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                textBox.SelectionLength = (int)e.NewValue;
            }
        }
    }
}