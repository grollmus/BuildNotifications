using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Configuration.Options;

/// <summary>
/// </summary>
[PublicAPI]
public class ListOptionItem<TValue>
    : IListOptionItem
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="value">Value of this item.</param>
    /// <param name="displayName">Text used for displaying this item.</param>
    /// <param name="localize">
    /// Indicates whether the display name should be localized before being
    /// displayed in the UI.
    /// </param>
    public ListOptionItem(TValue value, string displayName, bool localize = false)
    {
        Value = value;
        DisplayName = displayName;
        Localize = localize;
    }

    /// <summary>
    /// Text used used for displaying this item
    /// </summary>
    public string DisplayName { get; }

    /// <summary>
    /// Indicates whether the display name should be localized before being displayed in the UI.
    /// </summary>
    public bool Localize { get; }

    /// <summary>
    /// Value of this item
    /// </summary>
    public TValue Value { get; }

    string IListOptionItem.DisplayName => DisplayName;

    object? IListOptionItem.Value => Value;
}