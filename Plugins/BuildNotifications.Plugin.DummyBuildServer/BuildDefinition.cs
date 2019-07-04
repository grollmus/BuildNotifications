using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Plugin.DummyBuildServer
{
    internal class BuildDefinition : IBuildDefinition
    {
        /// <inheritdoc />
        public bool Equals(IBuildDefinition other)
        {
            return Id == (other as BuildDefinition)?.Id;
        }

        /// <inheritdoc />
        public string Id { get; set; }

        /// <inheritdoc />
        public string Name { get; set; }
    }
}