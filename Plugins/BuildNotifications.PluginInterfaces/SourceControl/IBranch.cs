using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.SourceControl
{
    /// <summary>
    /// Contains information about a branch.
    /// </summary>
    [PublicAPI]
    public interface IBranch
    {
        /// <summary>
        /// Name that can be used when displaying this branch.
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// The name of the branch.
        /// </summary>
        string Name { get; }
    }
}