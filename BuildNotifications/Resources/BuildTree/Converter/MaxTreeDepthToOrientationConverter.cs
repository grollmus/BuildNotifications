using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace BuildNotifications.Resources.BuildTree.Converter
{
    /// <summary>
    /// With zero, one and three levels of grouping the first level looks better when displayed horizontally.
    /// </summary>
    internal class MaxTreeDepthToOrientationConverter : IValueConverter
    {
        public static MaxTreeDepthToOrientationConverter Instance { get; } = new MaxTreeDepthToOrientationConverter();

        private MaxTreeDepthToOrientationConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int maxTreeDepth)
                return maxTreeDepth == 0 || maxTreeDepth % 2 == 1 ? Orientation.Horizontal : Orientation.Vertical;

            return Orientation.Vertical;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}