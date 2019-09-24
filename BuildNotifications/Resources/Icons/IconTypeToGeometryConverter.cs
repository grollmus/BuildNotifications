using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace BuildNotifications.Resources.Icons
{
    internal class IconTypeToGeometryConverter : IValueConverter
    {
        static IconTypeToGeometryConverter()
        {
            Cache = SupportedIcons.ToDictionary(x => x, IconTypeToGeometry);
        }

        private IconTypeToGeometryConverter()
        {
        }

        public static IconTypeToGeometryConverter Instance { get; } = new IconTypeToGeometryConverter();

        private static IEnumerable<IconType> SupportedIcons
        {
            get
            {
                yield return IconType.Connection;
                yield return IconType.Branch;
                yield return IconType.Definition;
                yield return IconType.Queued;
                yield return IconType.Person;
                yield return IconType.Settings;
            }
        }

        private static Geometry IconTypeToGeometry(IconType type)
        {
            switch (type)
            {
                case IconType.Connection:
                    return Geometry.Parse("F1 M12,12z M0,0z M9.45,2.6L7.9,2.6 7.9,0.05 6.85,0.05 6.85,2.6 5.15,2.6 5.15,0.05 4.15,0.05 4.15,2.6 2.65,2.6 2.6,2.6 2.6,6Q2.6,6.3 2.65,6.55 2.8,7.55 3.45,8.3 4.3,9.25 5.6,9.4L5.6,12 6.45,12 6.45,9.4Q7.75,9.25 8.6,8.3 9.45,7.3 9.45,6L9.45,2.6z");
                case IconType.Branch:
                    return Geometry.Parse("F1 M12,12z M0,0z M11.95,8.15Q11.95,7.3 11.3,6.65 10.65,6 9.8,6 9.1,6 8.45,6.55L4.2,2.75 4.25,2.2Q4.25,1.35 3.65,0.7 3.15,0.2 2.6,0.1 2.4,0.05 2.15,0.05 1.25,0.05 0.65,0.7 0,1.35 0,2.2 0,2.45 0.05,2.65 0.15,3.15 0.45,3.55 1,4.15 1.7,4.3L1.7,7.85Q1,7.95 0.45,8.55 0,9.1 0,9.9 0,10.75 0.65,11.4 1.25,12 2.15,12 2.4,12 2.6,11.95 3.15,11.85 3.65,11.4 4.25,10.75 4.25,9.9 4.25,9.1 3.75,8.55 3.3,7.95 2.6,7.85L2.55,7.85 2.55,4.3 2.6,4.3Q3.3,4.15,3.75,3.55L7.9,7.25Q7.7,7.65 7.7,8.15 7.7,9.05 8.35,9.7 8.95,10.3 9.8,10.3 10.65,10.3 11.3,9.7 11.95,9.05 11.95,8.15 M11.1,8.15Q11.1,8.7 10.7,9.05 10.3,9.45 9.8,9.45 9.25,9.45 8.95,9.05 8.55,8.7 8.55,8.15 8.55,7.65 8.95,7.3 9.25,6.9 9.8,6.9 10.3,6.9 10.7,7.3 11.1,7.65 11.1,8.15z");
                case IconType.Definition:
                    return Geometry.Parse("F1 M12,12z M0,0z M10.8,6.6Q11.95,5.45 11.95,3.9 11.95,3.2 11.6,2.25L9.2,4.65 7.35,2.85 9.75,0.45Q8.9,0.05 8.1,0.05 6.55,0.05 5.35,1.2 5.3,1.3 5.2,1.4 4.25,2.5 4.25,3.9 4.25,4.6 4.5,5.25 4.55,5.4 4.6,5.55L0.35,9.85Q0,10.2 0,10.75 0,11.25 0.35,11.7 0.7,12 1.3,12 1.85,12 2.2,11.7L6.45,7.4Q7.25,7.75 8.1,7.75 9.7,7.75 10.8,6.6z");
                case IconType.Queued:
                    return Geometry.Parse("F1 M12,12z M0,0z M1.8,-1.5L-1.7,-3.45 -1.7,0.45 1.8,2.7 1.8,-1.5 M6,-3.45L2.45,-1.5 2.45,2.7 6,0.45 6,-3.45 M5.8,-4.05L2.15,-6 -1.5,-4.05 2.15,-2.15 5.8,-4.05 M-5.85,-0.1Q-5.85,-0.15,-5.9,-0.15L-5.95,-0.15 -6,-0.1Q-6.05,0.9 -5.45,2.05 -4.2,4.25 -0.85,4.95L-0.9,6 2.5,4.05 -0.9,2.05 -0.85,3.15Q-1.7,3.25 -2.65,2.95 -4.9,2.3 -5.85,-0.1z");
                case IconType.Person:
                    return Geometry.Parse("F1 M12,12z M0,0z M3.6,6.2Q2.6,7.2,2.6,8.65L2.6,12.05 9.45,12.05 9.45,8.65Q9.45,7.2 8.45,6.2 7.45,5.2 6,5.2 4.6,5.2 3.6,6.2 M7.6,3.9Q8.25,3.25 8.25,2.3 8.25,1.35 7.6,0.7 6.95,0.05 6,0.05 5.05,0.05 4.4,0.7 3.75,1.35 3.75,2.3 3.75,3.25 4.4,3.9 5.05,4.55 6,4.55 6.95,4.55 7.6,3.9z");
                case IconType.Settings:
                    return Geometry.Parse("F1 M12,12z M0,0z M12,5.05L10.2,4.75 9.9,4.05 10.95,2.55 9.55,1.1 8.05,2.2 7.7,2 7.35,1.9 7.05,0.05 5,0.05 4.7,1.9 4,2.2 2.5,1.1 1.05,2.55 2.15,4.05 1.95,4.4 1.85,4.75 0,5.05 0,7.1 1.85,7.4 2.15,8.1 1.05,9.6 2.5,11 4,9.95 4.7,10.25 5,12.05 7.05,12.05 7.35,10.25 8.05,9.95 9.55,11 10.95,9.6 9.9,8.1 10.2,7.4 12,7.1 12,5.05 M7.35,4.7Q7.9,5.3 7.9,6.05 7.9,6.85 7.35,7.4 6.8,8 6.05,8 5.25,8 4.65,7.4 4.1,6.85 4.1,6.05 4.1,5.3 4.65,4.7 5.25,4.15 6.05,4.15 6.8,4.15 7.35,4.7z");
                default:
                    return Geometry.Empty;
            }
        }

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IconType type && Cache.ContainsKey(type))
                return Cache[type];

            return Geometry.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static readonly Dictionary<IconType, Geometry> Cache;
    }
}