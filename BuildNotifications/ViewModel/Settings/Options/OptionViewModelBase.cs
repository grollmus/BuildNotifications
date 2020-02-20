using System;

namespace BuildNotifications.ViewModel.Settings.Options
{
    public abstract class OptionViewModelBase : BaseViewModel
    {
        public event EventHandler? ValueChanged;

        protected void RaiseValueChanged()
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public abstract class OptionViewModelBase<TValue> : OptionViewModelBase
    {
        protected OptionViewModelBase(TValue value, string displayName)
        {
            _value = value;
            DisplayName = displayName;
        }

        public string DisplayName { get; }

        public TValue Value
        {
            get => _value;
            set
            {
                if (Equals(_value, value))
                    return;

                _value = value;
                OnPropertyChanged();
                RaiseValueChanged();
            }
        }

        private TValue _value;
    }
}