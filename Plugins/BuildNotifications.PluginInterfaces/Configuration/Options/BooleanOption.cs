using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Configuration.Options
{
    /// <summary>
    /// Option containing a boolean.
    /// </summary>
    [PublicAPI]
    public class BooleanOption : ValueOption<bool>
    {
        /// <inheritdoc cref="ValueOption{TValue}" />
        public BooleanOption(bool value, string nameTextId, string descriptionTextId)
            : base(value, nameTextId, descriptionTextId)
        {
        }
    }
}