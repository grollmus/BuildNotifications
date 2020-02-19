using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Configuration.Options
{
    /// <summary>
    /// Option containing an encryptable string.
    /// </summary>
    [PublicAPI]
    public class EncryptedTextOption : ValueOption<PasswordString?>
    {
        /// <inheritdoc />
        public EncryptedTextOption(string plainValue, string nameTextId, string descriptionTextId)
            : base(PasswordString.FromPlainText(plainValue), nameTextId, descriptionTextId)
        {
        }
    }
}