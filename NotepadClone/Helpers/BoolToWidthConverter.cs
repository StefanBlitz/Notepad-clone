using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NotepadClone.Helpers
{
    public class BoolToWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool visible = (bool)value;
            return visible ? new GridLength(250) : new GridLength(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}