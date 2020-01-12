using System;
using System.Windows.Input;

namespace BuildNotifications.ViewModel.Utils
{
    public class DelegateCommand : ICommand
    {
        public DelegateCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public DelegateCommand(Action<object> execute)
            : this(execute, x => true)
        {
        }

        public DelegateCommand(Action execute)
            : this(obj => execute(), x => true)
        {
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) == true;

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        private readonly Predicate<object> _canExecute;
        private readonly Action<object> _execute;
    }
}