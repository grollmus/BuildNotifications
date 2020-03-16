using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Configuration.Options
{
    /// <summary>
    /// Option that can be used to display text to the user.
    /// </summary>
    [PublicAPI]
    public class DisplayOption : ValueOption<string>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="value">Value to display.</param>
        /// <param name="nameTextId">Text id used for localizing the name of this option.</param>
        /// <param name="descriptionTextId">Text id used for localizing the description of this option.</param>
        public DisplayOption(string value, string nameTextId, string descriptionTextId)
            : base(value, nameTextId, descriptionTextId)
        {
        }
    }
}