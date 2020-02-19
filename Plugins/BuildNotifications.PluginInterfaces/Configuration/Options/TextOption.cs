using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Configuration.Options
{
    /// <summary>
    /// Option containing a text.
    /// </summary>
    [PublicAPI]
    public class TextOption : ValueOption<string?>
    {
        /// <inheritdoc cref="ValueOption{TValue}" />
        public TextOption(string value, string nameTextId, string descriptionTextId) : base(value, nameTextId, descriptionTextId)
        {
        }

        /// <summary>
        /// Maximum allowed length of the text.
        /// </summary>
        public virtual int MaximumLength => int.MaxValue;

        /// <summary>
        /// Minimum required length of the text.
        /// </summary>
        public virtual int MinimumLength => 0;

        /// <inheritdoc />
        protected override bool ValidateValue(string? value)
        {
            if (!base.ValidateValue(value))
                return false;

            var length = value?.Length ?? 0;

            return length <= MaximumLength && length >= MinimumLength;
        }
    }
}