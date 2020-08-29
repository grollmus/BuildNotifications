using System;
using System.Collections;
using System.Collections.Generic;
using BuildNotifications.PluginInterfacesLegacy.Notification;
using NLog.Fluent;

namespace BuildNotifications.Core.Pipeline.Notification.Distribution
{
    public abstract class BaseNotificationDistributor : INotificationDistributor
    {
        protected abstract IDistributedNotification ToDistributedNotification(INotification notification);

        public void Distribute(INotification notification)
        {
            Log.Info().Message($"Distributing notification \"{notification.GetType().Name}\".").Write();
            var distributedNotification = ToDistributedNotification(notification);
            foreach (var processor in this)
            {
                Log.Debug().Message($"Distributing notification \"{notification.GetType().Name}\" with processor \"{processor.GetType().Name}\".").Write();
                processor.Process(distributedNotification);
            }

            _distributedNotifications.Add(notification, distributedNotification);
        }

        public void ClearAllMessages()
        {
            foreach (var notification in _distributedNotifications)
            {
                foreach (var processor in _processors)
                {
                    processor.Clear(notification.Value);
                }
            }

            _distributedNotifications.Clear();
        }

        public void ClearDistributedMessage(INotification notification)
        {
            if (!_distributedNotifications.TryGetValue(notification, out var distributedNotification))
                return;

            foreach (var processor in _processors)
            {
                processor.Clear(distributedNotification);
            }

            _distributedNotifications.Remove(notification);
        }

        public IEnumerator<INotificationProcessor> GetEnumerator() => _processors.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(INotificationProcessor processor)
        {
            Log.Info().Message($"Adding NotificationProcessor \"{processor}\"").Write();
            try
            {
                Log.Debug().Message($"Calling Initialize on NotificationProcessor \"{processor}\"").Write();
                processor.Initialize();
            }
            catch (Exception e)
            {
                Log.Warn().Message($"NotificationProcessor \"{processor}\" failed to initialize. Error: \n {e}").Exception(e).Write();
            }

            Log.Debug().Message($"NotificationProcessor \"{processor}\" successfully initialized. Adding to list of processors.").Write();
            _processors.Add(processor);
        }

        public void Clear()
        {
            Log.Info().Message("Clearing all NotificationProcessors.").Write();
            foreach (var processor in _processors)
            {
                Log.Debug().Message($"Shutting down NotificationProcessor \"{processor}\"").Write();
                try
                {
                    processor.Shutdown();
                }
                catch (Exception e)
                {
                    Log.Warn().Message($"NotificationProcessor \"{processor}\" failed to shutdown. Error: \n {e}").Exception(e).Write();
                }
            }

            Log.Debug().Message("All NotificationProcessor are shut down. Clearing internal list of processors.").Write();
            _processors.Clear();
        }

        public bool Contains(INotificationProcessor item) => _processors.Contains(item);

        public void CopyTo(INotificationProcessor[] array, int arrayIndex)
        {
            _processors.CopyTo(array, arrayIndex);
        }

        public bool Remove(INotificationProcessor processor)
        {
            Log.Info().Message($"Clearing NotificationProcessors {processor}").Write();
            if (!Contains(processor))
            {
                Log.Debug().Message($"NotificationProcessor \"{processor}\" was not present within the list of processors.").Write();
                return false;
            }

            var result = _processors.Remove(processor);
            if (!result)
                Log.Debug().Message($"Removed NotificationProcessor \"{processor}\".").Write();
            else
                Log.Warn().Message($"Did not remove NotificationProcessor \"{processor}\" after checking if its present within the list. This might indicate concurrency problems.").Write();

            return result;
        }

        public int Count => _processors.Count;

        public bool IsReadOnly => false;
        private readonly Dictionary<INotification, IDistributedNotification> _distributedNotifications = new Dictionary<INotification, IDistributedNotification>();
        private readonly IList<INotificationProcessor> _processors = new List<INotificationProcessor>();
    }
}