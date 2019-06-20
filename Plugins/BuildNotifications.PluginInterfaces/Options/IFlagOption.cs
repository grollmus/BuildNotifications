using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Options
{
    /// <summary>
    /// An option with yes/no value.
    /// </summary>
    [PublicAPI]
    public interface IFlagOption : IOption
    {
        /// <summary>
        /// Default value of the option.
        /// </summary>
        bool DefaultValue { get; set; }

        /// <summary>
        /// The current value of this option.
        /// </summary>
        bool Value { get; set; }
    }
}