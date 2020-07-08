using System;
using System.Windows.Input;

namespace BuildNotifications.ViewModel.Utils
{
    public class DelegateCommand : DelegateCommand<object>
    {
        public DelegateCommand(Action execute)
            : this(execute, () => true)
        {
        }

        public DelegateCommand(Action execute, Func<bool> canExecute)
            : base(_ => execute(), _ => canExecute())
        {
        }
    }

    public class DelegateCommand<TParameter> : ICommand
    {
        public DelegateCommand(Action<TParameter> execute, Predicate<TParameter> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public DelegateCommand(Action<TParameter> execute)
            : this(execute, x => true)
        {
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// Parameter could be null, but is treated as a valid value. As we cannot differentiate between T = object and T = object? during runtime.
        /// </summary>
        public bool CanExecute(object parameter) => _canExecute.Invoke((TParameter) parameter);

        public void Execute(object parameter)
        {
            _execute((TParameter) parameter);
        }

        private readonly Predicate<TParameter> _canExecute;
        private readonly Action<TParameter> _execute;
    }
}