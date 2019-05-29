using BuildNotifications.PluginInterfaces.Builds;
using Microsoft.TeamFoundation.Build.WebApi;

namespace BuildNotifications.Plugin.Tfs
{
    internal class TfsBuildDefinition : IBuildDefinition
    {
        public TfsBuildDefinition(BuildDefinitionReference definition)
        {
            Id = definition.Id.ToString();
            Name = definition.Name;

            _url = definition.Url;
        }

        /// <inheritdoc />
        public bool Equals(IBuildDefinition definition)
        {
            var other = definition as TfsBuildDefinition;
            return _url == other?._url;
        }

        /// <inheritdoc />
        public string Id { get; }

        /// <inheritdoc />
        public string Name { get; }

        private string _url;
    }
}