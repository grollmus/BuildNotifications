using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Plugin.DummyBuildServer
{
    internal class Branch : IBranch
    {
        public Branch()
        {
        }

        public Branch(string name)
        {
            DisplayName = Name = name;
        }

        public override string ToString()
        {
            return DisplayName;
        }

        public bool Equals(IBranch other)
        {
            return Name == (other as Branch)?.Name;
        }

        public string DisplayName { get; set; }

        public string Name { get; set; }
    }
}