using System;
using System.Globalization;
using System.Windows.Data;
using Anotar.NLog;
using BuildNotifications.Core.Text;

namespace BuildNotifications.Resources.Text
{
    internal class StringKeyToLocalizedTextConverter : IValueConverter
    {
        public static StringKeyToLocalizedTextConverter Instance { get; } = new StringKeyToLocalizedTextConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var asString = value?.ToString();
            if (asString == null)
                return "";

            try
            {
                return StringLocalizer.Instance[asString];
            }
            catch (Exception)
            {
                LogTo.Warn("Failed to retrieve localized text for key: " + asString);
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
