using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using NLog.Fluent;

namespace BuildNotifications.Resources.Global
{
    public class BoolToBrushConverter : IValueConverter
    {
        public string BrushWhenTrue { get; set; } = "Background3";

        private SolidColorBrush GetBrush(string key)
        {
            if (Application.Current.FindResource(key) is not SolidColorBrush findResource)
            {
                Log.Debug().Message($"Resource {key} was not found. Stacktrace: \r\n{Environment.StackTrace}.").Write();
                return new SolidColorBrush(Colors.White);
            }

            return findResource;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool asBool && asBool)
                return GetBrush("Background3");
            return new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}