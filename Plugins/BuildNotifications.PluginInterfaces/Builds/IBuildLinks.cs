using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.PluginInterfaces.Builds
{
    /// <summary>
    /// Describes various related Links/URLs for a build.
    /// </summary>
    public interface IBuildLinks
    {
        /// <summary>
        /// Website URL for a branch definition.
        /// </summary>
        string? BranchWeb { get; }

        /// <summary>
        /// Website URL for a build.
        /// </summary>
        string? BuildWeb { get; }

        /// <summary>
        /// Website URL for a build definition.
        /// </summary>
        string? DefinitionWeb { get; }

        /// <summary>
        /// Update the URL for the branch with the specified branch.
        /// </summary>
        /// <param name="branch">Branch to use to update the URL.</param>
        void UpdateWith(IBranch branch);
    }
}