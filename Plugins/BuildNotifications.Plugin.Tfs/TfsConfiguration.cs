using JetBrains.Annotations;

namespace BuildNotifications.Plugin.Tfs
{
    [NoReorder]
    public class TfsConfiguration
    {
        public string? Url { get; set; }

        public string? CollectionName { get; set; }

        public string? ProjectId { get; set; }

        public string? RepositoryId { get; set; }

        public AuthenticationType AuthenticationType { get; set; }

        public string? Username { get; set; }

        public string? Password { get; set; }
    }

    public enum AuthenticationType
    {
        Windows,
        Account,
        Token
    }
}