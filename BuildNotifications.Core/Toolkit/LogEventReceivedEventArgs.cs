using System;
using NLog;

namespace BuildNotifications.Core.Toolkit
{
    public class LogEventReceivedEventArgs : EventArgs
    {
        public LogEventInfo LogEventInfo { get; }

        public LogEventReceivedEventArgs(LogEventInfo logEventInfo)
        {
            LogEventInfo = logEventInfo;
        }
    }
}