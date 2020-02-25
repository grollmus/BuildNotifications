using BuildNotifications.PluginInterfaces.Builds;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.VisualStudio.Services.WebApi;

namespace BuildNotifications.Plugin.Tfs.Build
{
    internal class TfsBuildDefinition : IBuildDefinition
    {
        public TfsBuildDefinition(DefinitionReference definition)
        {
            Name = definition.Name;
            NativeId = definition.Id;
            Id = NativeId.ToString();
            Links = new ReferenceLinks();
        }

        public TfsBuildDefinition(BuildDefinition definition)
            : this((DefinitionReference) definition)
        {
            Links = definition.Links;
        }

        public ReferenceLinks Links { get; }

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