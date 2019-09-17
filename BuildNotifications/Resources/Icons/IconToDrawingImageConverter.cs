using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace BuildNotifications.Resources.Icons
{
    internal class IconToDrawingImageConverter : IValueConverter
    {
        public static IconToDrawingImageConverter Instance { get; } = new IconToDrawingImageConverter();

        private IconToDrawingImageConverter()
        {
        }

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var key = $"{value}DrawingImage";

            return TryFindResource(Application.Current.MainWindow, key);
        }

        private readonly Dictionary<string, DrawingImage?> _cache = new Dictionary<string, DrawingImage?>();

        private DrawingImage? TryFindResource(FrameworkElement element, string key)
        {
            if (_cache.TryGetValue(key, out var existingTemplate))
                return existingTemplate;

            existingTemplate = element.TryFindResource(key) as DrawingImage;
            _cache.Add(key, existingTemplate);

            return existingTemplate;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}