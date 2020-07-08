using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Configuration.Options
{
    /// <summary>
    /// </summary>
    [PublicAPI]
    public interface IListOptionItem
    {
        /// <summary>
        /// Text used used for displaying this item
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Value of this item
        /// </summary>
        object? Value { get; }
    }
}