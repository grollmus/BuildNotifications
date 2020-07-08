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
                var clamped = Clamp(value);
                if (Equals(_value, clamped))
                    return;

                if (!ValidateValue(clamped))
                    return;

                _value = clamped;
                RaiseValueChanged();
            }
        }

        /// <summary>
        /// Called during Value setter to clamp the value to a valid range.
        /// </summary>
        /// <remarks>
        /// Either override <see cref="Clamp" /> to implicitly set valid values
        /// or override <see cref="ValidateValue" /> to forbid setting invalid values.
        /// </remarks>
        /// <param name="value">Value that is to be set</param>
        /// <returns>The clamped value.</returns>
        protected virtual TValue Clamp(TValue value) => value;

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
        /// <remarks>
        /// Either override <see cref="Clamp" /> to implicitly set valid values
        /// or override <see cref="ValidateValue" /> to forbid setting invalid values.
        /// </remarks>
        /// <param name="value">The value that is about to be set.</param>
        /// <returns><c>true</c> if the value is valid for this option; otherwise <c>false</c>.</returns>
        protected virtual bool ValidateValue(TValue value) => true;

        /// <inheritdoc />
        public virtual event EventHandler<EventArgs>? ValueChanged;

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