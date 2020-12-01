using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using BuildNotifications.Core.Toolkit;
using BuildNotifications.ViewModel.Utils;
using NLog.Fluent;
using Timer = System.Timers.Timer;

namespace BuildNotifications.ViewModel.Overlays.Toolkit
{
    internal class ToolkitViewModel : OverlayViewModel
    {
        private readonly Timer _timer;

        public RemoveTrackingObservableCollection<PluginTestActionViewModel> PluginTests { get; } = new RemoveTrackingObservableCollection<PluginTestActionViewModel>();

        public ICommand CloseCommand { get; }

        public ToolkitViewModel()
        {
            PluginTestActionFactory pluginTestActionFactory = new PluginTestActionFactory();
            CloseCommand = new DelegateCommand(() =>
            {
                _timer.Enabled = false;
                RequestClose(false);
            });
            PluginTests.Add(new PluginTestActionViewModel(pluginTestActionFactory.CreateTest(DoSomething, "Do something")));

            _timer = new Timer(1000);
            SetupTimer();
        }

        private void UpdateLogTimes(object? sender, ElapsedEventArgs elapsedEventArgs)
        {
            foreach (var pluginTest in PluginTests)
            {
                pluginTest.UpdateLogTimes();
            }
        }

        private void SetupTimer()
        {
            _timer.Elapsed += UpdateLogTimes;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        private async Task DoSomething(CancellationToken cancelToken)
        {
            await Task.Delay(1000, cancelToken);
            Log.Info().Message("Some info").Write();
            await Task.Delay(1000, cancelToken);
            throw new NotImplementedException("asd");
        }
    }
}