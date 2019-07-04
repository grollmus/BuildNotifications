using BuildNotifications.PluginInterfaces;

namespace BuildNotifications.Plugin.DummyBuildServer
{
    public class User : IUser
    {
        public User(string id, string uniqueName, string displayName)
        {
            Id = id;
            UniqueName = uniqueName;
            DisplayName = displayName;
        }

        /// <inheritdoc />
        public string DisplayName { get; }

        /// <inheritdoc />
        public string Id { get; }

        /// <inheritdoc />
        public string UniqueName { get; }
    }
}