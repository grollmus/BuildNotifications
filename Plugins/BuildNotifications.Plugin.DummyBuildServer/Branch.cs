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

        /// <inheritdoc />
        public override string ToString()
        {
            return DisplayName;
        }

        /// <inheritdoc />
        public bool Equals(IBranch other)
        {
            return Name == (other as Branch)?.Name;
        }

        /// <inheritdoc />
        public string DisplayName { get; set; }

        /// <inheritdoc />
        public string Name { get; set; }
    }
}