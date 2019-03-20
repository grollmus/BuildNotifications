using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Options
{
    /// <summary>
    /// An option containing a text value.
    /// </summary>
    [PublicAPI]
    public interface ITextOption : IOption
    {
        /// <summary>
        /// Default value of the option.
        /// </summary>
        string DefaultValue { get; set; }
    }
}