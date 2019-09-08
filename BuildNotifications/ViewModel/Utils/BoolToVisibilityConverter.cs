using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BuildNotifications.ViewModel.Utils
{
    internal class BoolToVisibilityConverter : IValueConverter
    {
        public static BoolToVisibilityConverter Instance { get; } = new BoolToVisibilityConverter(false);

        public static BoolToVisibilityConverter Inverted { get; } = new BoolToVisibilityConverter(true);

        private readonly bool _isInverted;

        private BoolToVisibilityConverter(bool isInverted)
        {
            _isInverted = isInverted;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool asBool))
                return Visibility.Visible;

            if (_isInverted)
                return asBool ? Visibility.Collapsed : Visibility.Visible;
            else
                return asBool ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}