using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.ViewModel.Tree;

namespace BuildNotifications.Resources.BuildTree.Converter
{
    internal class BuildTreeNodeMultiBindingToBrushConverter : IMultiValueConverter
    {
        public static BuildTreeNodeMultiBindingToBrushConverter Instance { get; } = new BuildTreeNodeMultiBindingToBrushConverter();

        private BuildTreeNodeMultiBindingToBrushConverter()
        {
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var node = values.OfType<BuildTreeNodeViewModel>().FirstOrDefault();
            if (node != null)
                return BuildStatusToBrushConverter.Instance.Convert(node);

            var buildStatus = values.OfType<BuildStatus>().ToList();
            if (buildStatus.Any())
                return BuildStatusToBrushConverter.Instance.Convert(buildStatus.First());

            return BuildStatusToBrushConverter.Instance.Convert(null);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}