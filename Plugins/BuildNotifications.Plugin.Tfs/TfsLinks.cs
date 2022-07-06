using BuildNotifications.Plugin.Tfs.Build;
using BuildNotifications.Plugin.Tfs.SourceControl;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;
using Microsoft.VisualStudio.Services.WebApi;

namespace BuildNotifications.Plugin.Tfs;

internal class TfsLinks : IBuildLinks
{
    public TfsLinks(Microsoft.TeamFoundation.Build.WebApi.Build fromBuild)
    {
        BuildWeb = TryGetLink(fromBuild.Links, "web");
    }

    public void UpdateLinks(TfsBuildDefinition definition)
    {
        DefinitionWeb = TryGetLink(definition.Links, "web");
    }

    private string? TryGetLink(ReferenceLinks referenceLinks, string key) => referenceLinks.Links.TryGetValue(key, out var link)
        ? ((ReferenceLink)link).Href
        : null;

    public string? BuildWeb { get; }

    public string? BranchWeb { get; private set; }

    public string? DefinitionWeb { get; private set; }

    public void UpdateWith(IBranch branch)
    {
        if (branch is not TfsBranch tfsBranch)
            return;

        BranchWeb = tfsBranch.WebUrl;
    }
}