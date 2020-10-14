using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using BuildNotifications.Core.Pipeline.Notification;
using NLog.Fluent;

namespace BuildNotifications.ViewModel.Utils
{
    internal class NotificationTypeToBrushConverter : IValueConverter
    {
        private NotificationTypeToBrushConverter()
        {
        }

        public SolidColorBrush DefaultBrush => GetBrush(DefaultBrushKey);

        public static NotificationTypeToBrushConverter Instance { get; } = new NotificationTypeToBrushConverter();

        public Brush Convert(NotificationType status)
        {
            switch (status)
            {
                case NotificationType.None:
                    return GetBrush("Foreground1");
                case NotificationType.Debug:
                    return GetBrush("Gray");
                case NotificationType.Info:
                    return GetBrush("DarkBlue");
                case NotificationType.Success:
                    return GetBrush("Green");
                case NotificationType.Warning:
                    return GetBrush("Yellow");
                case NotificationType.Error:
                    return GetBrush("Red");
                default:
                    return GetBrush("Background3");
            }
        }

        private SolidColorBrush GetBrush(string key)
        {
            if (_cache.TryGetValue(key, out var cachedBrush))
                return cachedBrush;

            if (!(Application.Current.FindResource(key) is SolidColorBrush resolvedBrush))
            {
                Log.Debug().Message($"Resource {key} was not found. Stacktrace: \r\n{Environment.StackTrace}.").Write();
                return new SolidColorBrush(Colors.White);
            }

            _cache.Add(key, resolvedBrush);
            return resolvedBrush;
        }

        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case NotificationType status:
                    return Convert(status);
                default:
                    return DefaultBrush;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

        private readonly Dictionary<string, SolidColorBrush> _cache = new Dictionary<string, SolidColorBrush>();

        private const string DefaultBrushKey = "Foreground1";
    }
}