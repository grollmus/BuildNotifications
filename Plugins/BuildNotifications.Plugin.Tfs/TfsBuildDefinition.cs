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
        }

        internal int NativeId { get; }

        public bool Equals(IBuildDefinition definition)
        {
            var other = definition as TfsBuildDefinition;
            return NativeId == other?.NativeId;
        }

        public string Id { get; }

        public string Name { get; }
    }
}