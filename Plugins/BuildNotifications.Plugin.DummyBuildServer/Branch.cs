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
            DisplayName = FullName = name;
        }

        public override string ToString() => DisplayName;

        public bool Equals(IBranch other) => FullName == (other as Branch)?.FullName;

        public string DisplayName { get; set; }

        public string FullName { get; set; }
        public bool IsPullRequest => false;
    }
}