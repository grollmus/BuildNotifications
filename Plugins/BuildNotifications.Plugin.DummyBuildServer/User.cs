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

        public User()
        {
        }

        public User(string name)
        {
            Id = DisplayName = UniqueName = name;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return DisplayName;
        }

        /// <inheritdoc />
        public string DisplayName { get; set; }

        /// <inheritdoc />
        public string Id { get; set; }

        /// <inheritdoc />
        public string UniqueName { get; set; }
    }
}