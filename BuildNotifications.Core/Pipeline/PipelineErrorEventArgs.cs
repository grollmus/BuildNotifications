using System;

namespace BuildNotifications.Core.Pipeline
{
    public class PipelineErrorEventArgs : EventArgs
    {
        public Exception Exception { get; }

        public string MessageTextId { get; }

        public string[] MessageParameter { get; }

        public PipelineErrorEventArgs(Exception exception, string messageTextId, string[] messageParameter)
        {
            Exception = exception;
            MessageTextId = messageTextId;
            MessageParameter = messageParameter;
        }
    }
}