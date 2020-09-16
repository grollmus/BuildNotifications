using System;
using JetBrains.Annotations;

namespace BuildNotifications.Core.Pipeline
{
    /// <summary>
    /// Defines how a pipeline should update.
    /// </summary>
    [Flags]
    [PublicAPI]
    public enum UpdateModes
    {
        /// <summary>
        /// Fetch new builds since last update.
        /// </summary>
        DeltaBuilds = 0x1,

        /// <summary>
        /// Fetch all builds.
        /// </summary>
        AllBuilds = 0x2,
    }
}