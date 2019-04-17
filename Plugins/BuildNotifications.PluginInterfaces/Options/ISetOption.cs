using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Options
{
    /// <summary>
    /// An option that has a set of predefined valid values.
    /// </summary>
    [PublicAPI]
    public interface ISetOption : IOption
    {
        /// <summary>
        /// Default value of the option.
        /// </summary>
        ISetItem? DefaultValue { get; set; }

        /// <summary>
        /// Set of valid values for this option.
        /// </summary>
        ISetItem[] Values { get; set; }
    }
}