using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.ViewModel.Tree;
using NLog.Fluent;

namespace BuildNotifications.Resources.BuildTree.Converter
{
    internal class BuildStatusToBrushConverter : IValueConverter
    {
        private BuildStatusToBrushConverter()
        {
        }

        public SolidColorBrush DefaultBrush => GetBrush(DefaultBrushKey);

        public static BuildStatusToBrushConverter Instance { get; } = new BuildStatusToBrushConverter();

        public Brush Convert(BuildStatus status)
        {
            switch (status)
            {
                case BuildStatus.None:
                    return GetBrush("Foreground1");
                case BuildStatus.Cancelled:
                    return GetBrush("Gray");
                case BuildStatus.Pending:
                    return GetBrush("DarkBlue");
                case BuildStatus.Succeeded:
                    return GetBrush("Green");
                case BuildStatus.PartiallySucceeded:
                    return GetBrush("Yellow");
                case BuildStatus.Failed:
                    return GetBrush("Red");
                case BuildStatus.Running:
                    return GetBrush("Blue");
                default:
                    return GetBrush("Background3");
            }
        }

        public Brush Convert(BuildTreeNodeViewModel node)
        {
            if (node.ShouldColorByStatus)
                return Convert(node.BuildStatus);
            return DefaultBrush;
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
                case BuildTreeNodeViewModel node:
                    return Convert(node);
                case BuildStatus status:
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