using System;

namespace BuildNotifications.Core.Pipeline
{
    /// <summary>
    /// Raises notifications when the pipeline has processed new items.
    /// </summary>
    public interface IPipelineNotifier
    {
        /// <summary>
        /// Raised when the pipeline has been updated.
        /// </summary>
        event EventHandler<PipelineUpdateEventArgs> Updated;
        
        /// <summary>
        /// Raised when an error during an Pipeline update event occured.
        /// </summary>
        event EventHandler<PipelineErrorEventArgs> ErrorOccured;
    }
}