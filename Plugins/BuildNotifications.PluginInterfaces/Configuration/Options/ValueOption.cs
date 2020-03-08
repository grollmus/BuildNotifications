using System;
using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Configuration.Options
{
    /// <summary>
    /// Base class for a value option.
    /// </summary>
    [PublicAPI]
    public abstract class ValueOption<TValue> : Option, IValueOption
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="value">Initial value of the this option.</param>
        /// <param name="nameTextId">Text id used for localizing the name of this option.</param>
        /// <param name="descriptionTextId">Text id used for localizing the description of this option.</param>
        protected ValueOption(TValue value, string nameTextId, string descriptionTextId)
            : base(nameTextId, descriptionTextId)
        {
            _value = value;
        }

        /// <summary>
        /// Current value of this option.
        /// </summary>
        public TValue Value
        {
            get => _value;
            set
            {
                if (Equals(_value, value))
                    return;

                if (!ValidateValue(value))
                    return;

                var oldValue = _value;
                _value = value;
                RaiseValueChanged(oldValue, value);
            }
        }

        object? IValueOption.Value
        {
            get => Value;
            set
            {
                if (value is TValue typed)
                    Value = typed;
                else
                    throw new ArgumentException();
            }
        }

        /// <summary>
        /// Raised when the value of this option has changed.
        /// </summary>
        public virtual event EventHandler<ValueChangedEventArgs<TValue>>? ValueChanged;

        /// <summary>
        /// Raises the ValueChanged event.
        /// </summary>
        /// <param name="oldValue">The previous value of this option.</param>
        /// <param name="newValue">The new value of this option.</param>
        protected void RaiseValueChanged(TValue oldValue, TValue newValue)
        {
            ValueChanged?.Invoke(this, new ValueChangedEventArgs<TValue>(oldValue, newValue));
        }

        /// <summary>
        /// Called during Value setter to check if new value is valid.
        /// </summary>
        /// <param name="value">The value that is about to be set.</param>
        /// <returns><c>true</c> if the value is valid for this option; otherwise <c>false</c>.</returns>
        protected virtual bool ValidateValue(TValue value)
        {
            return true;
        }

        private TValue _value;
    }
}