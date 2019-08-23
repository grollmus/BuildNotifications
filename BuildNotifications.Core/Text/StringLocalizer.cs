using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

            foreach (var culture in LocalizedCultures())
            {
                var resourceSet = _resourceManager.GetResourceSet(culture, true, true);
                var resourceDictionary = resourceSet.Cast<DictionaryEntry>()
                    .ToDictionary(r => r.Key.ToString(), r => r.Value.ToString());

                Cache.Add(culture, resourceDictionary);
            }

            _defaultDictionary = Cache[DefaultCulture];
        }

        public static CultureInfo DefaultCulture => CultureInfo.GetCultureInfo("en-US");
        public static StringLocalizer Instance { get; } = new StringLocalizer();

        public string this[string key] => GetText(key);

        public IDictionary<CultureInfo, IDictionary<string, string>> Cache { get; set; } = new Dictionary<CultureInfo, IDictionary<string, string>>();

        private readonly IDictionary<string, string> _defaultDictionary;

        public string GetText(string key, CultureInfo culture = null)
        {
            if (key == null)
                return "";

            if (culture == null)
                culture = CultureInfo.CurrentUICulture;

            if (!Cache.TryGetValue(culture, out var dictionary))
            {
                if (!Cache.TryGetValue(culture.Parent, out dictionary))
                    dictionary = _defaultDictionary;
            }

            if (!dictionary.TryGetValue(key, out var localizedText))
            {
                if (!_defaultDictionary.TryGetValue(key, out localizedText))
                    localizedText = key;
            }

            return localizedText;
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

        private static IEnumerable<CultureInfo> LocalizedCultures()
        {
            yield return CultureInfo.GetCultureInfo("en-US");
            yield return CultureInfo.GetCultureInfo("de");
        }

        private readonly ResourceManager _resourceManager;
    }
}