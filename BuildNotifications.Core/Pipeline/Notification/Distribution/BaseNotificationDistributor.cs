using System;
using System.Collections;
using System.Collections.Generic;
using Anotar.NLog;
using BuildNotifications.PluginInterfacesLegacy.Notification;

namespace BuildNotifications.Core.Pipeline.Notification.Distribution
{
    public abstract class BaseNotificationDistributor : INotificationDistributor
    {
        protected abstract IDistributedNotification ToDistributedNotification(INotification notification);

        public void Distribute(INotification notification)
        {
            LogTo.Info($"Distributing notification \"{notification.GetType().Name}\".");
            foreach (var processor in this)
            {
                LogTo.Debug($"Distributing notification \"{notification.GetType().Name}\" with processor \"{processor.GetType().Name}\".");
                processor.Process(ToDistributedNotification(notification));
            }
        }

        public IEnumerator<INotificationProcessor> GetEnumerator()
        {
            return _processors.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(INotificationProcessor processor)
        {
            if (processor == null)
                return;

            LogTo.Info($"Adding NotificationProcessor \"{processor}\"");
            try
            {
                LogTo.Debug($"Calling Initialize on NotificationProcessor \"{processor}\"");
                processor.Initialize();
            }
            catch (Exception e)
            {
                LogTo.WarnException($"NotificationProcessor \"{processor}\" failed to initialize. Error: \n {e}", e);
            }

            LogTo.Debug($"NotificationProcessor \"{processor}\" successfully initialized. Adding to list of processors.");
            _processors.Add(processor);
        }

        public void Clear()
        {
            LogTo.Info("Clearing all NotificationProcessors.");
            foreach (var processor in _processors)
            {
                LogTo.Debug($"Shutting down NotificationProcessor \"{processor}\"");
                try
                {
                    processor.Shutdown();
                }
                catch (Exception e)
                {
                    LogTo.WarnException($"NotificationProcessor \"{processor}\" failed to shutdown. Error: \n {e}", e);
                }
            }

            LogTo.Debug("All NotificationProcessor are shut down. Clearing internal list of processors.");
            _processors.Clear();
        }

        public bool Contains(INotificationProcessor item)
        {
            return _processors.Contains(item);
        }

        public void CopyTo(INotificationProcessor[] array, int arrayIndex)
        {
            _processors.CopyTo(array, arrayIndex);
        }

        public bool Remove(INotificationProcessor processor)
        {
            LogTo.Info($"Clearing NotificationProcessors {processor}");
            if (!Contains(processor))
            {
                LogTo.Debug($"NotificationProcessor \"{processor}\" was not present within the list of processors.");
                return false;
            }

            var result = _processors.Remove(processor);
            if (!result)
                LogTo.Debug($"Removed NotificationProcessor \"{processor}\".");
            else
                LogTo.Warn($"Did not remove NotificationProcessor \"{processor}\" after checking if its present within the list. This might indicate concurrency problems.");

            return result;
        }

        public int Count => _processors.Count;

        public bool IsReadOnly => false;
        private readonly IList<INotificationProcessor> _processors = new List<INotificationProcessor>();
    }
}