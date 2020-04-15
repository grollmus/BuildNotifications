using System;
using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.SourceControl
{
    /// <summary>
    /// Contains information about a branch.
    /// </summary>
    [PublicAPI]
    public interface IBranch : IEquatable<IBranch>
    {
        /// <summary>
        /// Name that can be used when displaying this branch.
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// The name of the branch.
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// Indicates whether this branch is a real branch (false) or one that was
        /// created during/for a pull request (true);
        /// </summary>
        bool IsPullRequest { get; }
    }
}