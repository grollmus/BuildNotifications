using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace BuildNotifications.Resources.Icons
{
    class IconTypeToGeometryConverter : IValueConverter
    {
        public static IconTypeToGeometryConverter Instance {get;} = new IconTypeToGeometryConverter();

        private IconTypeToGeometryConverter()
        {
            
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Geometry.Parse("F1 M-6,-6z M6,6z M58.4,13.35L50.95,5.6 42.95,11.45 41.05,10.4 39.2,9.85 37.6,0 26.65,0 25.05,9.85 21.35,11.45 13.35,5.6 5.6,13.35 11.45,21.35 10.4,23.2 9.85,25.05 0,26.65 0,37.6 9.85,39.2 11.45,42.95 5.6,50.95 13.35,58.4 21.35,52.8 25.05,54.4 26.65,64 37.6,64 39.2,54.4 42.95,52.8 50.95,58.4 58.4,50.95 52.8,42.95 54.4,39.2 64,37.6 64,26.65 54.4,25.05 52.8,21.35 58.4,13.35 M21.85,32Q21.85,28 24.8,24.8 28,21.85 32.25,21.85 36.25,21.85 39.2,24.8 42.15,28 42.15,32 42.15,36.25 39.2,39.2 36.25,42.4 32.25,42.4 28,42.4 24.8,39.2 21.85,36.25 21.85,32z");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
