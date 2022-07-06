using System;
using Microsoft.TeamFoundation.Core.WebApi;

namespace BuildNotifications.Plugin.Tfs;

public class TfsProject
{
    public TfsProject(TeamProjectReference project)
    {
        ProjectName = project.Name;
        Id = project.Id.ToString();
    }

    public TfsProject()
    {
        Id = string.Empty;
    }

    public string Id { get; set; }

    public string? ProjectName { get; set; }

    public override bool Equals(object? obj)
    {
        var other = obj as TfsProject;
        return other?.Id.Equals(Id, StringComparison.InvariantCulture) == true;
    }

    public override int GetHashCode() =>
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        Id.GetHashCode(StringComparison.InvariantCulture);

    public override string ToString() => ProjectName ?? string.Empty;
}