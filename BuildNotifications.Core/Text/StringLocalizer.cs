using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using Anotar.NLog;

namespace BuildNotifications.Core.Text
{
    /// <summary>
    /// Retrieves localized strings from the Texts.resx files
    /// </summary>
    public class StringLocalizer
    {
        private StringLocalizer()
        {
            var resourceManager = new ResourceManager("BuildNotifications.Core.Text.Texts", Assembly.GetAssembly(typeof(StringLocalizer)));
            _resourceManager = resourceManager;
        }

        public static CultureInfo DefaultCulture => CultureInfo.GetCultureInfo("en-US");
        public static StringLocalizer Instance { get; } = new StringLocalizer();

        public string this[string key] => GetText(key);

        public string GetText(string key, CultureInfo culture = null)
        {
            if (key == null)
                return "";

            if (culture == null)
                culture = CultureInfo.CurrentUICulture;

            try
            {
                return _resourceManager.GetString(key, culture);
            }
            catch (Exception)
            {
                try
                {
                    return _resourceManager.GetString(key, DefaultCulture);
                }
                catch (Exception)
                {
                    LogTo.Warn($"Failed to retrieve localized text for key: \"{key}\"");
                    return key;
                }
            }
        }

        public static IEnumerable<CultureInfo> SupportedCultures()
        {
            yield return CultureInfo.GetCultureInfo("en-US");
            yield return CultureInfo.GetCultureInfo("de");
            yield return CultureInfo.GetCultureInfo("de-LU");
            yield return CultureInfo.GetCultureInfo("de-LI");
            yield return CultureInfo.GetCultureInfo("de-IT");
            yield return CultureInfo.GetCultureInfo("de-DE");
            yield return CultureInfo.GetCultureInfo("de-BE");
            yield return CultureInfo.GetCultureInfo("de-AT");
            yield return CultureInfo.GetCultureInfo("de-CH");
        }

        private readonly ResourceManager _resourceManager;
    }
}