using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BuildNotifications.ViewModel.Utils.Converters
{
    internal class BoolToVisibilityConverter : IValueConverter
    {
        private BoolToVisibilityConverter(bool isInverted)
        {
            _isInverted = isInverted;
        }

        public static BoolToVisibilityConverter Instance { get; } = new BoolToVisibilityConverter(false);

        public static BoolToVisibilityConverter Inverted { get; } = new BoolToVisibilityConverter(true);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool asBool)
                return Visibility.Visible;

            if (_isInverted)
                return asBool ? Visibility.Collapsed : Visibility.Visible;
            return asBool ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private readonly bool _isInverted;
    }
}