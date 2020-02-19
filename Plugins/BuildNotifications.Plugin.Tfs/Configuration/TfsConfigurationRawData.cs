using BuildNotifications.PluginInterfaces;

namespace BuildNotifications.Plugin.Tfs.Configuration
{
    internal class TfsConfigurationRawData
    {
        public AuthenticationType AuthenticationType { get; set; }
        public string CollectionName { get; set; } = string.Empty;
        public PasswordString? Password { get; set; }
        public TfsProject? Project { get; set; }
        public TfsRepository? Repository { get; set; }
        public PasswordString? Token { get; set; }
        public string? Url { get; set; }
        public string? Username { get; set; }
    }
}