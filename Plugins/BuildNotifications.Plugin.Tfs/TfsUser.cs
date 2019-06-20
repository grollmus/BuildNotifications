using BuildNotifications.PluginInterfaces;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;

namespace BuildNotifications.Plugin.Tfs
{
    internal class TfsUser : IUser
    {
        public TfsUser(Identity identity)
        {
            DisplayName = identity.DisplayName;
            Id = identity.Id.ToString();
            UniqueName = identity.Descriptor.Identifier;
        }

        public TfsUser(IdentityRef identity)
        {
            DisplayName = identity.DisplayName;
            Id = identity.Id;
            UniqueName = identity.Descriptor.Identifier;
        }

        /// <inheritdoc />
        public string DisplayName { get; }

        /// <inheritdoc />
        public string Id { get; }

        /// <inheritdoc />
        public string UniqueName { get; }
    }
}