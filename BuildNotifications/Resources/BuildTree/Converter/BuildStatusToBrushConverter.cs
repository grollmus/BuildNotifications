using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Anotar.NLog;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.ViewModel.Tree;

namespace BuildNotifications.Resources.BuildTree.Converter
{
    internal class BuildStatusToBrushConverter : IValueConverter
    {
        private BuildStatusToBrushConverter()
        {
        }

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
                    return GetBrush("Blue");
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
            if (node != null && node.ShouldColorByStatus)
                return Convert(node.BuildStatus);
            return DefaultBrush;
        }

        public Brush DefaultBrush => GetBrush(DefaultBrushKey);

        private Brush GetBrush(string key)
        {
            var findResource = Application.Current.FindResource(key) as Brush;
            if (findResource == null)
            {
                LogTo.Debug($"Resource {key} was not found. Stacktrace: \r\n{Environment.StackTrace}.");
                return new SolidColorBrush(Colors.White);
            }

            return findResource;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
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

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private const string DefaultBrushKey = "Foreground1";
    }
}