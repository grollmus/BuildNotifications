using BuildNotifications.PluginInterfaces.Builds;
using Microsoft.TeamFoundation.Build.WebApi;

namespace BuildNotifications.Plugin.Tfs
{
    internal class TfsBuildDefinition : IBuildDefinition
    {
        public TfsBuildDefinition(DefinitionReference definition)
        {
            Name = definition.Name;
            NativeId = definition.Id;
            Id = NativeId.ToString();

            _url = definition.Url;
        }

        internal int NativeId { get; }

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