using BuildNotifications.PluginInterfaces.Builds;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.VisualStudio.Services.WebApi;

namespace BuildNotifications.Plugin.Tfs
{
    internal class TfsLinks : IBuildLinks
    {
        public TfsLinks(Build fromBuild)
        {
            BuildWeb = TryGetLink(fromBuild, "web");
        }

        private string? TryGetLink(Build fromBuild, string key)
        {
            if (fromBuild.Links.Links.TryGetValue(key, out var link))
                return ((ReferenceLink) link).Href;
            return null;
        }

        public string? BuildWeb { get; }

        public string? BranchWeb => null;

        public string? DefinitionWeb => null;
    }
}