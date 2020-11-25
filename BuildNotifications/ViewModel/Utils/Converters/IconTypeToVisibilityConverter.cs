using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using BuildNotifications.Resources.Icons;

namespace BuildNotifications.ViewModel.Utils.Converters
{
    internal class IconTypeToVisibilityConverter : IValueConverter
    {
        public static IconTypeToVisibilityConverter Instance { get; } = new IconTypeToVisibilityConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IconType icon)
            {
                if (icon == IconType.None)
                    return Visibility.Collapsed;

                return Visibility.Visible;
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}