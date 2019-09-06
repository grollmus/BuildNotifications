using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace BuildNotifications.Core
{
    public class SynchronizationContextRemover : INotifyCompletion
    {
        public bool IsCompleted => SynchronizationContext.Current == null;

        public SynchronizationContextRemover GetAwaiter()
        {
            return this;
        }

        public void GetResult()
        {
            // Only exists because this class needs to be awaited.
        }

        /// <inheritdoc />
        public void OnCompleted(Action continuation)
        {
            var prevContext = SynchronizationContext.Current;
            try
            {
                SynchronizationContext.SetSynchronizationContext(null);
                continuation();
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(prevContext);
            }
        }
    }
}