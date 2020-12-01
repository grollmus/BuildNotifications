using NLog;

namespace BuildNotifications.Core.Toolkit
{
    public interface ILogReceiver
    {
        void Write(LogEventInfo logEvent);
    }
}