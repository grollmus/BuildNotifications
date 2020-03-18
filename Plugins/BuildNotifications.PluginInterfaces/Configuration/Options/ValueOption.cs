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

                _value = value;
                RaiseValueChanged();
            }
        }

        /// <summary>
        /// Raised when the value of this option has changed.
        /// </summary>
        public virtual event EventHandler<EventArgs>? ValueChanged;

        /// <summary>
        /// Raises the ValueChanged event.
        /// </summary>
        protected void RaiseValueChanged()
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
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

        object? IValueOption.Value
        {
            get => Value;
            set
            {
                if (value is TValue typed)
                    Value = typed;
                else
                    throw new ArgumentException("Value has wrong type", nameof(value));
            }
        }

        private TValue _value;
    }
}