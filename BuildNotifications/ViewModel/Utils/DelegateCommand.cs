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

        public bool CanExecute(object parameter) => parameter is TParameter asT && _canExecute.Invoke(asT);

        public void Execute(object parameter)
        {
            _execute((TParameter) parameter);
        }

        private readonly Predicate<TParameter> _canExecute;
        private readonly Action<TParameter> _execute;
    }
}