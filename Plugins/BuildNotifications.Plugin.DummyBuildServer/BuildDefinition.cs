using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Plugin.DummyBuildServer
{
    internal class BuildDefinition : IBuildDefinition
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

        /// <inheritdoc />
        public bool Equals(IBuildDefinition other)
        {
            return Id == (other as BuildDefinition)?.Id;
        }

        /// <inheritdoc />
        public string Id { get; set; }

        /// <inheritdoc />
        public string Name { get; set; }

        private static int _idCounter;
    }
}