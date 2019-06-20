using System;
using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Builds
{
    /// <summary>
    /// Contains information about a single build.
    /// </summary>
    [PublicAPI]
    public interface IBuild : IEquatable<IBuild>
    {
        /// <summary>
        /// Name of the branch the build was for.
        /// </summary>
        string BranchName { get; }

        /// <summary>
        /// Build definition used for this build.
        /// </summary>
        IBuildDefinition Definition { get; }

        /// <summary>
        /// Unique id of this build.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Date and time this build was changed the last time.
        /// </summary>
        DateTime? LastChangedTime { get; }

        /// <summary>
        /// Date and time this build was added to the build queue.
        /// </summary>
        DateTime? QueueTime { get; }

        /// <summary>
        /// User that requested this build.
        /// </summary>
        IUser RequestedBy { get; }

        /// <summary>
        /// User this been has been requested for.
        /// </summary>
        IUser? RequestedFor { get; }

        /// <summary>
        /// Status of this build.
        /// </summary>
        BuildStatus Status { get; }
    }
}