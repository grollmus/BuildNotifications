using System.Collections.Generic;
using System.Globalization;
using BuildNotifications.Core.Text;

namespace BuildNotifications.ViewModel.Settings.Options;

public class LanguageOptionViewModel : ListOptionBaseViewModel<CultureInfo>
{
    public LanguageOptionViewModel(string language)
        : base(StringLocalizer.Keys.Language, CultureInfo.CreateSpecificCulture(language))
    {
    }

    protected override IEnumerable<CultureInfo> ModelValues
    {
        get
        {
            yield return CultureInfo.CreateSpecificCulture("en-US");
            yield return CultureInfo.CreateSpecificCulture("de");
        }
    }
}