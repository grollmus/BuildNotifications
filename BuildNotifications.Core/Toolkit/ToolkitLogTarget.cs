using System;
using System.Collections.Generic;
using NLog;
using NLog.Targets;

namespace BuildNotifications.Core.Toolkit
{
    [Target("ToolkitLog")]
    public sealed class ToolkitLogTarget : TargetWithLayout
    {
        private static readonly IList<ILogReceiver> LogReceivers = new List<ILogReceiver>();

        protected override void Write(LogEventInfo logEvent)
        {
            foreach (var logReceiver in LogReceivers)
            {
                logReceiver.Write(logEvent);
            }
        }

        public static IDisposable ReceiveLogs(ILogReceiver target)
        {
            LogReceivers.Add(target);

            return new LogRun(target);
        }

        private static void Deregister(ILogReceiver logReceiver) => LogReceivers.Remove(logReceiver);

        private class LogRun : IDisposable
        {
            private readonly ILogReceiver _logReceiver;

            public LogRun(ILogReceiver logReceiver)
            {
                _logReceiver = logReceiver;
            }

            public void Dispose()
            {
                ToolkitLogTarget.Deregister(_logReceiver);
            }
        }
    }
}