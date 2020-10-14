using System;
using BuildNotifications.Core.Pipeline.Notification;
using BuildNotifications.Resources.Icons;
using BuildNotifications.ViewModel.Utils;
using NLog;

namespace BuildNotifications.ViewModel.Overlays.Toolkit
{
    internal class LogEventInfoViewModel : BaseViewModel
    {
        private readonly LogEventInfo _logEventInfo;

        public string Content => _logEventInfo.FormattedMessage;

        public IconType IconType => ResolveIconType();

        public bool IsUnread
        {
            get => _isUnread;
            set
            {
                _isUnread = value;
                OnPropertyChanged();
            }
        }

        public NotificationType NotificationType => ResolveNotificationType();

        public string Source => _logEventInfo.LoggerName ?? string.Empty;

        public DateTime Timestamp { get; }

        public TimeSpan TimeUntilNow => Timestamp.TimespanToNow();

        public string DisplayTimestamp => Timestamp.ToString("dd-MM-yyyy hh:mm:ss.fff tt");

        public string LogLevel => _logEventInfo.Level?.ToString() ?? "Unknown";

        public void InvokeTimeUntilNowUpdate()
        {
            OnPropertyChanged(nameof(TimeUntilNow));
        }

        private NotificationType ResolveNotificationType()
        {
            return _logEventInfo.Level.Name switch
            {
                "Trace" => NotificationType.None,
                "Debug" => NotificationType.Debug,
                "Info" => NotificationType.Info,
                "Warn" => NotificationType.Warning,
                "Error" => NotificationType.Error,
                "Fatal" => NotificationType.Error,
                _ => NotificationType.None
            };
        }

        private IconType ResolveIconType()
        {
            return _logEventInfo.Level.Name switch
            {
                "Trace" => IconType.Dummy,
                "Debug" => IconType.Dummy,
                "Info" => IconType.Info,
                "Warn" => IconType.Lightning,
                "Error" => IconType.Lightning,
                "Fatal" => IconType.Lightning,
                _ => IconType.None
            };
        }

        private bool _isUnread = true;

        public LogEventInfoViewModel(LogEventInfo logEventInfo)
        {
            _logEventInfo = logEventInfo;
            Timestamp = DateTime.Now;
        }
    }
}