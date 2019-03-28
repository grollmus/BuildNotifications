using BuildNotifications.Core.Config;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Core.Pipeline
{
    /// <summary>
    /// A project is a combination of a BuildProvider and SourceControlProvider
    /// that can be used to gather information about a single software project.
    /// </summary>
    internal interface IProject
    {
        /// <summary>
        /// Branch provider that is used to fetch branch information for this project.
        /// </summary>
        IBranchProvider BranchProvider { get; }

        /// <summary>
        /// Build provider that is used to fetch build information for this project.
        /// </summary>
        IBuildProvider BuildProvider { get; }

        /// <summary>
        /// Configuration for this project.
        /// </summary>
        IProjectConfiguration Config { get; }
    }
}