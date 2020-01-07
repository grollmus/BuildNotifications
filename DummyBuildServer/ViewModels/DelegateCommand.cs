using System;
using System.Windows.Input;

namespace DummyBuildServer.ViewModels
{
    internal class DelegateCommand : DelegateCommand<object>
    {
        public DelegateCommand(Action<object> executeAction, Func<object, bool> canExecuteFunc)
            : base(executeAction, canExecuteFunc)
        {
        }

        public DelegateCommand(Action<object> executeAction)
            : base(executeAction)
        {
        }
    }

    internal class DelegateCommand<TParam> : ICommand
    {
        public DelegateCommand(Action<TParam> executeAction, Func<TParam, bool> canExecuteFunc)
        {
            _executeAction = executeAction;
            _canExecuteFunc = canExecuteFunc;
        }

        public DelegateCommand(Action<TParam> executeAction)
            : this(executeAction, x => true)
        {
        }

        private event EventHandler? CanExecuteChangedInternal;

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChangedInternal?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            return _canExecuteFunc((TParam) parameter);
        }

        public void Execute(object parameter)
        {
            if (!_canExecuteFunc((TParam) parameter))
                return;

            _executeAction((TParam) parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
                CanExecuteChangedInternal += value;
            }

            remove
            {
                CommandManager.RequerySuggested -= value;
                CanExecuteChangedInternal -= value;
            }
        }

        private readonly Action<TParam> _executeAction;
        private readonly Func<TParam, bool> _canExecuteFunc;
    }
}