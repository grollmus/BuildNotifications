using System;
using BuildNotifications.Core.Text;

namespace BuildNotifications.ViewModel.Settings.Options
{
    public abstract class OptionViewModelBase : BaseViewModel
    {
        public OptionViewModelBase(string displayName, string description)
        {
            DisplayName = StringLocalizer.Instance.GetText(displayName);
            Description = StringLocalizer.Instance.GetText(description);
        }

        public virtual string Description { get; }
        public virtual string DisplayName { get; }

        public virtual bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled == value)
                    return;

                _isEnabled = value;
                OnPropertyChanged();
            }
        }

        public virtual bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading == value)
                    return;

                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public virtual bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible == value)
                    return;

                _isVisible = value;
                OnPropertyChanged();
            }
        }

        public event EventHandler? ValueChanged;

        protected void RaiseValueChanged()
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private bool _isEnabled = true;
        private bool _isLoading;
        private bool _isVisible = true;
    }

    public abstract class OptionViewModelBase<TValue> : OptionViewModelBase
    {
        protected OptionViewModelBase(TValue value, string displayName)
            : base(displayName, string.Empty)
        {
            _value = value;
        }

        public virtual TValue Value
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