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
        IBranchProvider BranchProvider { get; }
        IBuildProvider BuildProvider { get; }
    }
}