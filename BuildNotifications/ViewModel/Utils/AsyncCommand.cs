using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BuildNotifications.ViewModel.Utils
{
    public static class AsyncCommand
    {
        public static AsyncCommand<object?> Create(Func<Task> command, Func<bool> canExecute)
        {
            return new AsyncCommand<object?>(async _ =>
            {
                await command();
                return null;
            }, canExecute);
        }

        public static AsyncCommand<object?> Create(Func<Task> command)
        {
            return Create(command, () => true);
        }

        public static AsyncCommand<TResult> Create<TResult>(Func<Task<TResult>> command)
        {
            return new AsyncCommand<TResult>(_ => command());
        }

        public static AsyncCommand<object?> Create(Func<CancellationToken, Task> command)
        {
            return new AsyncCommand<object?>(async token =>
            {
                await command(token);
                return null;
            });
        }

        public static AsyncCommand<TResult> Create<TResult>(Func<CancellationToken, Task<TResult>> command) => new AsyncCommand<TResult>(command);
    }

    public class AsyncCommand<TResult> : IAsyncCommand, INotifyPropertyChanged
    {
        public AsyncCommand(Func<CancellationToken, Task<TResult>> command)
            : this(command, () => true)
        {
        }

        public AsyncCommand(Func<CancellationToken, Task<TResult>> command, Func<bool> canExecute)
        {
            _command = command;
            _canExecute = canExecute;
            _cancelCommand = new CancelAsyncCommand();
        }

        public ICommand CancelCommand => _cancelCommand;

        public NotifyTaskCompletion<TResult>? Execution
        {
            get => _execution;
            private set
            {
                _execution = value;
                OnPropertyChanged();
            }
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        public bool CanExecute(object parameter) => Execution == null && _canExecute() || Execution == null || Execution.IsCompleted;

        public async Task ExecuteAsync(object parameter)
        {
            _cancelCommand.NotifyCommandStarting();
            Execution = new NotifyTaskCompletion<TResult>(_command(_cancelCommand.Token));
            RaiseCanExecuteChanged();
            await Execution.TaskCompletion;
            _cancelCommand.NotifyCommandFinished();
            RaiseCanExecuteChanged();
        }

        public async void Execute(object parameter)
        {
            await ExecuteAsync(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly Func<CancellationToken, Task<TResult>> _command;
        private readonly Func<bool> _canExecute;
        private readonly CancelAsyncCommand _cancelCommand;
        private NotifyTaskCompletion<TResult>? _execution;

        private sealed class CancelAsyncCommand : ICommand
        {
            public CancellationToken Token => _cts.Token;

            public void NotifyCommandFinished()
            {
                _commandExecuting = false;
                RaiseCanExecuteChanged();
            }

            public void NotifyCommandStarting()
            {
                _commandExecuting = true;
                if (!_cts.IsCancellationRequested)
                    return;
                _cts = new CancellationTokenSource();
                RaiseCanExecuteChanged();
            }

            private void RaiseCanExecuteChanged()
            {
                CommandManager.InvalidateRequerySuggested();
            }

            bool ICommand.CanExecute(object parameter) => _commandExecuting && !_cts.IsCancellationRequested;

            void ICommand.Execute(object parameter)
            {
                _cts.Cancel();
                RaiseCanExecuteChanged();
            }

            public event EventHandler CanExecuteChanged
            {
                add => CommandManager.RequerySuggested += value;
                remove => CommandManager.RequerySuggested -= value;
            }

            private CancellationTokenSource _cts = new CancellationTokenSource();
            private bool _commandExecuting;
        }
    }
}