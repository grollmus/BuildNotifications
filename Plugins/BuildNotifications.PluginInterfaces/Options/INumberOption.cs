using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Options
{
    /// <summary>
    /// An option containing an integer value.
    /// </summary>
    [PublicAPI]
    public interface INumberOption : IOption
    {
        /// <summary>
        /// Default value of the option.
        /// </summary>
        int DefaultValue { get; set; }

        /// <summary>
        /// Minimal allowed value for this option or <c>null</c> if no minimum exists.
        /// </summary>
        int? MinValue { get; set; }

        /// <summary>
        /// Maximal allowed value for this option or <c>null</c> if no maximum exists.
        /// </summary>
        int? MaxValue { get; set; }
    }
}