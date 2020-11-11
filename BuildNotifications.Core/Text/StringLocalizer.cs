using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace BuildNotifications.Core.Text
{
    /// <summary>
    /// Retrieves localized strings from the Texts.resx files
    /// </summary>
    public partial class StringLocalizer : IStringLocalizer
    {
        private StringLocalizer()
        {
            var assembly = Assembly.GetAssembly(typeof(StringLocalizer)) ?? Assembly.GetExecutingAssembly();
            var resourceManager = new ResourceManager("BuildNotifications.Core.Text.Texts", assembly)
            {
                IgnoreCase = true
            };

            foreach (var culture in LocalizedCultures())
            {
                var resourceSet = resourceManager.GetResourceSet(culture, true, true);
                if (resourceSet != null)
                {
                    var resourceDictionary = resourceSet.Cast<DictionaryEntry>()
                        .Where(r => !string.IsNullOrEmpty(r.Value?.ToString()))
                        .ToDictionary(r => r.Key.ToString()!, r => r.Value?.ToString() ?? string.Empty);

                    Cache.Add(culture, resourceDictionary);
                }
            }

            _defaultDictionary = Cache[DefaultCulture];
        }

        public static CultureInfo CurrentCulture => CultureInfo.CurrentUICulture;

        public static CultureInfo DefaultCulture => CultureInfo.GetCultureInfo("en-US");
        public static StringLocalizer Instance { get; } = new StringLocalizer();

        public string this[string key] => GetText(key);

        internal IDictionary<CultureInfo, IDictionary<string, string>> Cache { get; } = new Dictionary<CultureInfo, IDictionary<string, string>>();

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

        public string GetText(string key, CultureInfo? culture = null)
        {
            if (string.IsNullOrEmpty(key))
                return "";

            culture ??= CurrentCulture;

            if (!Cache.TryGetValue(culture, out var dictionary) && !Cache.TryGetValue(culture.Parent, out dictionary))
                dictionary = _defaultDictionary;

            if (!dictionary!.TryGetValue(key, out var localizedText) && !_defaultDictionary.TryGetValue(key, out localizedText))
                localizedText = key;

            return localizedText!;
        }

        private readonly IDictionary<string, string> _defaultDictionary;
    }
}