using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Options
{
    /// <summary>
    /// An item of a SetOption
    /// </summary>
    [PublicAPI]
    public interface ISetItem
    {
        /// <summary>
        /// A detailed description of the item.
        /// </summary>
        string? Description { get; set; }

        /// <summary>
        /// Name of the item.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Value of the item.
        /// </summary>
        object? Value { get; set; }
    }
}