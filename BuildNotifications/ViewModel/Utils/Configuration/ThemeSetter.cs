using System;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using BuildNotifications.Core.Config;
using NLog.Fluent;

namespace BuildNotifications.ViewModel.Utils.Configuration;

internal class ThemeSetter
{
    public ThemeSetter(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Apply()
    {
        Log.Debug().Message("Changing application theme").Write();

        var applicationResources = Application.Current.Resources;
        var themeDictionaryIndex = IndexOfCurrentThemeDictionary(applicationResources);
        if (themeDictionaryIndex == -1)
            return;

        var newThemeDictionary = LoadThemeDictionary(_configuration.ApplicationTheme);
        if (newThemeDictionary?.Count > 0)
            applicationResources.MergedDictionaries[themeDictionaryIndex] = newThemeDictionary;
    }

    private static string ExtractResourceDictionaryPath(Uri baseUri)
    {
        var toSkip = baseUri.Segments.TakeWhile(s => !s.Contains(';', StringComparison.OrdinalIgnoreCase)).Count() + 1;

        return string.Join('/', baseUri.Segments.Skip(toSkip));
    }

    private static int IndexOfCurrentThemeDictionary(ResourceDictionary resDict)
    {
        var darkTheme = resDict.MergedDictionaries.FirstOrDefault(d => IsSameSource(d, DarkThemeSource));
        var lightTheme = resDict.MergedDictionaries.FirstOrDefault(d => IsSameSource(d, LightThemeSource));

        if (darkTheme != null)
            return resDict.MergedDictionaries.IndexOf(darkTheme);
        if (lightTheme != null)
            return resDict.MergedDictionaries.IndexOf(lightTheme);

        Log.Error().Message("Found no theme dictionary in application resources").Write();
        return -1;
    }

    private static bool IsSameSource(ResourceDictionary dictionary, string xaml)
    {
        string sourceString;

        if (dictionary.Source != null)
            sourceString = dictionary.Source.OriginalString;
        else if (dictionary is IUriContext uriContext)
            sourceString = ExtractResourceDictionaryPath(uriContext.BaseUri);
        else
            return false;

        var sourceParts = sourceString.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
        var xamlParts = xaml.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);

        return sourceParts.SequenceEqual(xamlParts, StringComparer.OrdinalIgnoreCase);
    }

    private static ResourceDictionary? LoadThemeDictionary(Theme theme)
    {
        Uri source;

        switch (theme)
        {
            case Theme.Dark:
                source = PackUri(DarkThemeSource);
                break;
            case Theme.Light:
                source = PackUri(LightThemeSource);
                break;
            default:
                return new ResourceDictionary();
        }

        return Application.LoadComponent(source) as ResourceDictionary;
    }

    private static Uri PackUri(string path) => new($"/BuildNotifications;component/{path}", UriKind.RelativeOrAbsolute);

    private readonly IConfiguration _configuration;

    private const string DarkThemeSource = @"Resources\Global\DarkTheme.xaml";
    private const string LightThemeSource = @"Resources\Global\LightTheme.xaml";
}