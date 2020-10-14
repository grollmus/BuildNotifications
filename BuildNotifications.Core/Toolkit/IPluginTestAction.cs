using System;
using System.Threading.Tasks;

namespace BuildNotifications.Core.Toolkit
{
    public interface IPluginTestAction
    {
        Task Execute();

        event EventHandler<LogEventReceivedEventArgs>? LogEventReceived;

        string Name { get; }

        void Stop();
    }
}