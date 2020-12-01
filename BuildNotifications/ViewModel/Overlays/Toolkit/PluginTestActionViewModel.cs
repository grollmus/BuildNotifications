using System.Threading.Tasks;
using System.Windows.Input;
using BuildNotifications.Core.Text;
using BuildNotifications.Core.Toolkit;
using BuildNotifications.Resources.Icons;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel.Overlays.Toolkit
{
    internal class PluginTestActionViewModel : BaseViewModel
    {
        private readonly IPluginTestAction _action;

        public LogViewModel Logs { get; }

        public DelegateCommand ExecuteCommand { get; }

        public ICommand ClearCommand { get; }

        public string Name { get; }

        public IconType CommandIcon => IsRunning ? IconType.Pause : IconType.Play;

        public PluginTestActionViewModel(IPluginTestAction action)
        {
            _action = action;
            Name = _action.Name;
            Logs = new LogViewModel(action);
            ExecuteCommand = new DelegateCommand(ExecuteOrStop);
            ClearCommand = new DelegateCommand(() => Logs.Entries.Clear());
        }

        private Task? _runningTask;

        public bool IsRunning => _runningTask != null;

        private Task? RunningTask
        {
            get => _runningTask;
            set
            {
                _runningTask = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsRunning));
                OnPropertyChanged(nameof(CommandIcon));
                OnPropertyChanged(nameof(ExecuteToolTip));
                OnPropertyChanged(nameof(ExecuteText));
                ExecuteCommand.RaiseCanExecuteChanged();
            }
        }

        public string ExecuteToolTip => IsRunning ? StringLocalizer.StopTestTooltip : StringLocalizer.ExecuteTestTooltip;
        
        public string ExecuteText => IsRunning ? StringLocalizer.StopTestAction : StringLocalizer.ExecuteTestAction;

        private void ExecuteOrStop()
        {
            if (RunningTask != null)
            {
                _action.Stop();
                RunningTask = null;
                return;
            }

            Logs.Entries.Clear();
            RunningTask = RunAction();
        }

        private async Task RunAction()
        {
            using (GlobalErrorLogTarget.Mute())
            {
                await _action.Execute();

                RunningTask = null;
            }
        }

        public void UpdateLogTimes()
        {
            foreach (var logEntry in Logs.Entries)
            {
                logEntry.InvokeTimeUntilNowUpdate();
            }
        }
    }
}