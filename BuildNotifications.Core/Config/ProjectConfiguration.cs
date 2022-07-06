using System.Collections.Generic;
using BuildNotifications.Core.Text;

namespace BuildNotifications.Core.Config;

public class ProjectConfiguration : IProjectConfiguration
{
    public string SourceControlConnectionName { get; set; } = string.Empty;

    public bool IsEnabled { get; set; } = true;

    public string ProjectName { get; set; } = StringLocalizer.NewProject;

    public IReadOnlyList<string> BuildConnectionNames { get; set; } = new List<string>();

    public IReadOnlyList<string> BranchBlacklist { get; set; } = new List<string>();

    public IReadOnlyList<string> BranchWhitelist { get; set; } = new List<string>();

    public IReadOnlyList<string> BuildDefinitionBlacklist { get; set; } = new List<string>();

    public IReadOnlyList<string> BuildDefinitionWhitelist { get; set; } = new List<string>();

    public string DefaultCompareBranch { get; set; } = string.Empty;

    public bool HideCompletedPullRequests { get; set; } = true;

    public bool HideBuildsOfDeletedBranches { get; set; } = true;

    public bool HideBuildsOfDeletedDefinitions { get; set; } = true;

    public PullRequestDisplayMode PullRequestDisplay { get; set; } = PullRequestDisplayMode.Number;
}