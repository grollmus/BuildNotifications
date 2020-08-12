using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Plugin.DummyServer
{
    public class BuildDefinition : IBuildDefinition
    {
        public BuildDefinition()
            : this(string.Empty)
        {
        }

        public BuildDefinition(string name)
        {
            Name = name;
            Id = (++_idCounter).ToString();
        }

        public bool Equals(IBuildDefinition other)
        {
            return Id == (other as BuildDefinition)?.Id;
        }

        public string Id { get; set; }

        public string Name { get; set; }

        private static int _idCounter;
    }
}