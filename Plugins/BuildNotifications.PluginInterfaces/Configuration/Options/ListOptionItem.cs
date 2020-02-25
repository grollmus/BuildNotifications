using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Configuration.Options
{
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
        public ListOptionItem(TValue value, string displayName)
        {
            Value = value;
            DisplayName = displayName;
        }

        /// <summary>
        /// Text used used for displaying this item
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Value of this item
        /// </summary>
        public TValue Value { get; }

        string IListOptionItem.DisplayName => DisplayName;

        object? IListOptionItem.Value => Value;
    }
}