using System;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using NLog.Fluent;

namespace BuildNotifications.Core.Toolkit
{
    internal class PluginTestAction : IPluginTestAction, ILogReceiver, IDisposable
    {
        private readonly Func<CancellationToken, Task> _taskCreationFunction;

        public string Name { get; }

        public PluginTestAction(Func<CancellationToken, Task> taskCreationFunction, string name)
        {
            _taskCreationFunction = taskCreationFunction;
            Name = name;
        }

        public async Task Execute()
        {
            _logReceiver?.Dispose();
            _logReceiver = ToolkitLogTarget.ReceiveLogs(this);
            using (_logReceiver)
            {
                try
                {
                    Log.Debug().Message($"Executing \"{Name}\"").Write();
                    var task = _taskCreationFunction(_tokenSource.Token);
                    await task;
                }
                catch (TaskCanceledException e)
                {
                    Log.Info().Exception(e).Message($"Plugin test task \"{Name}\" was cancelled. ").Write();
                }
                catch (Exception e)
                {
                    Log.Error().Exception(e).Message($"An error occurred while running plugin test \"{Name}\". ").Write();
                }
                finally
                {
                    Log.Debug().Message($"Execution of \"{Name}\" finished").Write();
                }
            }
        }

        private CancellationTokenSource _tokenSource = new CancellationTokenSource();

        private IDisposable? _logReceiver;

        public void Stop()
        {
            _logReceiver?.Dispose();
            _tokenSource.Cancel();

            _tokenSource = new CancellationTokenSource();
        }

        public event EventHandler<LogEventReceivedEventArgs>? LogEventReceived;

        public void Write(LogEventInfo logEvent)
        {
            LogEventReceived?.Invoke(this, new LogEventReceivedEventArgs(logEvent));
        }

        public void Dispose()
        {
            _tokenSource.Dispose();
            _logReceiver?.Dispose();
        }
    }
}