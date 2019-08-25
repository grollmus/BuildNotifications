using System;
using System.Collections.Generic;
using BuildNotifications.Core.Pipeline.Notification;
using BuildNotifications.Core.Pipeline.Tree;

namespace BuildNotifications.Core.Pipeline
{
    internal class PipelineNotifier : IPipelineNotifier
    {
        public void Notify(IBuildTree tree, IEnumerable<INotification> delta)
        {
            Updated?.Invoke(this, new PipelineUpdateEventArgs(tree, delta));
        }

        private readonly IList<PipelineErrorEventArgs> _storedErrors = new List<PipelineErrorEventArgs>();

        /// <summary>
        /// Stores an error eventArgs instance for later release.
        /// </summary>
        public void StoreError(Exception exception, string messageTextId, params string[] messageParameter)
        {
            _storedErrors.Add(new PipelineErrorEventArgs(new ErrorNotification(messageTextId, messageParameter, exception)));
        }

        public void NotifyErrors()
        {
            foreach (var error in _storedErrors)
            {
                ErrorOccured?.Invoke(this, error);
            }

            _storedErrors.Clear();
        }

        public event EventHandler<PipelineUpdateEventArgs> Updated;

        public event EventHandler<PipelineErrorEventArgs> ErrorOccured;
    }
}