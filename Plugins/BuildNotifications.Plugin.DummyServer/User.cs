using BuildNotifications.PluginInterfaces;

namespace BuildNotifications.Plugin.DummyServer
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

        public override string ToString()
        {
            return DisplayName;
        }

        public string DisplayName { get; set; }

        public string Id { get; set; }

        public string UniqueName { get; set; }
    }
}