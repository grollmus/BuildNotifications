using System.Collections.Generic;

namespace BuildNotifications.ViewModel.Settings.Options;

public class StringCollectionOptionViewModel : CollectionOptionBaseViewModel<string?, TextOptionViewModel>
{
    public StringCollectionOptionViewModel(string displayName, IEnumerable<string> value)
        : base(value, displayName)
    {
    }

    protected override TextOptionViewModel CreateNewValue() => new(string.Empty, string.Empty);
    protected override TextOptionViewModel CreateNewValue(string? value) => new(value, value ?? string.Empty);
}