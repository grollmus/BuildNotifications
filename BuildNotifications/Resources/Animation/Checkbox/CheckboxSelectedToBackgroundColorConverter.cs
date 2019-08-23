using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace BuildNotifications.Resources.Animation.Checkbox
{
    internal class CheckboxSelectedToBackgroundColorConverter : IValueConverter
    {
        public static SolidColorBrush UncheckedBrush { get; }
        public static SolidColorBrush CheckedBrush { get; }

        public static CheckboxSelectedToBackgroundColorConverter Instance { get; } = new CheckboxSelectedToBackgroundColorConverter();

        static CheckboxSelectedToBackgroundColorConverter()
        {
            CheckedBrush = (SolidColorBrush) Application.Current.TryFindResource("Green");
            UncheckedBrush = (SolidColorBrush) Application.Current.TryFindResource("Red");
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool asBool))
                return new Thickness(0);

            return asBool ? CheckedBrush : UncheckedBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}