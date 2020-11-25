using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

namespace BuildNotifications.ViewModel.Utils.Converters
{
    internal class DebugBreakConverter : IValueConverter
    {
        public static DebugBreakConverter Instance { get; } = new DebugBreakConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Debugger.Break();
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Debugger.Break();
            return value;
        }
    }
}