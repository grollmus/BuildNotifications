using System.Security;
using JetBrains.Annotations;
using ReflectSettings.Attributes;

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

        [CalculatedVisibility(nameof(UsernameHidden))]
        public string? Username { get; set; }

        [CalculatedVisibility(nameof(PasswordHidden))]
        public string? Password { get; set; }

        [CalculatedVisibility(nameof(TokenHidden))]
        public string? Token { get; set; }

        [CalculatedVisibility(nameof(PasswordHidden))]
        public SecureString? SecurePassword { get; set; }

        public bool UsernameHidden() => AuthenticationType != AuthenticationType.Account;

        public bool PasswordHidden() => AuthenticationType != AuthenticationType.Account;

        public bool TokenHidden() => AuthenticationType != AuthenticationType.Token;
    }

    public enum AuthenticationType
    {
        Windows,
        Account,
        Token
    }
}