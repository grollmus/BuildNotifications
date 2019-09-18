using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BuildNotifications.Resources.Settings
{
    public class InvertedBooleanToVisibilityConverter : IValueConverter
    {
        private InvertedBooleanToVisibilityConverter()
        {
        }

        public static InvertedBooleanToVisibilityConverter Instance { get; } = new InvertedBooleanToVisibilityConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool asBool)
                return asBool ? Visibility.Collapsed : Visibility.Visible;

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}