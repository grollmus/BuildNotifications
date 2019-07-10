using System;
using System.Globalization;
using System.Windows.Data;
using Anotar.NLog;
using BuildNotifications.Core.Text;

namespace BuildNotifications.Resources.Text
{
    internal class EnumNameToLocalizedTextConverter : IValueConverter
    {
        private EnumNameToLocalizedTextConverter()
        {
        }

        public static EnumNameToLocalizedTextConverter Instance { get; } = new EnumNameToLocalizedTextConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "";

            try
            {
                var name = Enum.GetName(value.GetType(), value);
                return StringLocalizer.Instance[name];
            }
            catch (Exception)
            {
                LogTo.Warn("Failed to retrieve enum constant value from type: " + value.GetType());
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}