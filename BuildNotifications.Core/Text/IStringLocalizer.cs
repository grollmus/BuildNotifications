using System.Globalization;

namespace BuildNotifications.Core.Text;

public interface IStringLocalizer
{
    string GetText(string key, CultureInfo? culture = null);
}