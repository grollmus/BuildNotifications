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
            resourceManager.IgnoreCase = true;
            _resourceManager = resourceManager;
            var resourceSet = _resourceManager.GetResourceSet(CultureInfo.GetCultureInfo("en"), false, false);
        }

        public static CultureInfo DefaultCulture => CultureInfo.GetCultureInfo("en-US");
        public static StringLocalizer Instance { get; } = new StringLocalizer();

        public string this[string key] => GetText(key);

        public IDictionary<CultureInfo, IDictionary<string, string>> Cache { get; set; } = new Dictionary<CultureInfo, IDictionary<string, string>>();

        public string GetText(string key, CultureInfo culture = null)
        {
            if (key == null)
                return "";

            if (culture == null)
                culture = CultureInfo.CurrentUICulture;

            if (TryCached(key, culture, out var localizedText))
                return localizedText;

            try
            {
                localizedText = _resourceManager.GetString(key, culture);
            }
            catch (Exception)
            {
                try
                {
                    localizedText = _resourceManager.GetString(key, DefaultCulture);
                }
                catch (Exception)
                {
                    LogTo.Warn($"Failed to retrieve localized text for key: \"{key}\"");
                    localizedText = key;
                }
            }

            StoreInCache(key, culture, localizedText);
            return localizedText;
        }

        private bool TryCached(string key, CultureInfo culture, out string cachedEntry)
        {
            cachedEntry = key;
            if (!Cache.ContainsKey(culture))
                return false;

            if (Cache[culture].TryGetValue(key, out var result))
            {
                cachedEntry = result;
                return true;
            }

            return false;
        }

        private void StoreInCache(string key, CultureInfo culture, string localizedText)
        {
            if (!Cache.ContainsKey(culture))
                Cache.Add(culture, new Dictionary<string, string>());

            if (!Cache[culture].ContainsKey(key))
                Cache[culture].Add(key, localizedText);
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