using System;

namespace BuildNotifications.ViewModel.Settings.Options;

public class ListOptionItemViewModel<TItem> : BaseViewModel
{
    public ListOptionItemViewModel(TItem value, string? name)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
        DisplayName = name ?? value.ToString() ?? string.Empty;
    }

    public string DisplayName { get; }
    public TItem Value { get; }
}