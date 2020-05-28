using System;
using BuildNotifications.PluginInterfaces.SourceControl;
using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Builds
{
    /// <summary>
    /// Contains enriched information about a single build.
    /// </summary>
    [PublicAPI]
    public interface 
        IBuild : IBaseBuild
    {
        /// <summary>
        /// Name of the project this build originates from.
        /// </summary>
        string ProjectName { get; }

        /// <summary>
        /// ID of the project this build originates from.
        /// </summary>
        Guid ProjectId { get; }

        /// <summary>
        /// Whether this build has been requested by the current user identity of this builds source build provider
        /// </summary>
        bool IsRequestedByCurrentUser { get; }

        /// <summary>
        /// Branch this build originates from.
        /// </summary>
        IBranch? Branch { get; }
    }

    /// <summary>
    /// Contains information about a single build.
    /// </summary>
    [PublicAPI]
    public interface IBaseBuild : IEquatable<IBaseBuild>
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
        /// Date and time (in UTC) this build was changed the last time.
        /// </summary>
        DateTime? LastChangedTime { get; }

        /// <summary>
        /// Links (URLs) associated with this Build.
        /// </summary>
        IBuildLinks Links { get; }

        /// <summary>
        /// Progress in percent of this build.
        /// </summary>
        int Progress { get; }

        /// <summary>
        /// Date and time (in UTC) this build was added to the build queue.
        /// </summary>
        DateTime? QueueTime { get; }

        /// <summary>
        /// User that requested this build.
        /// </summary>
        IUser RequestedBy { get; }

        /// <summary>
        /// User this build has been requested for.
        /// </summary>
        IUser? RequestedFor { get; }

        /// <summary>
        /// Status of this build.
        /// </summary>
        BuildStatus Status { get; }

        /// <summary>
        /// Creation reason of the build.
        /// </summary>
        BuildReason Reason { get; }
    }
}