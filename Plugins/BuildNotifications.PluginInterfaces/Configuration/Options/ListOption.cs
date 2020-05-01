using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Configuration.Options
{
    /// <summary>
    /// An option containing a list of items to chose from.
    /// </summary>
    [PublicAPI]
    public abstract class ListOption<TValue> : ValueOption<TValue>, IListOption
    {
        /// <inheritdoc />
        protected ListOption(TValue value, string nameTextId, string descriptionTextId)
            : base(value, nameTextId, descriptionTextId)
        {
        }

        /// <summary>
        /// List of all available values for this option.
        /// </summary>
        public abstract IEnumerable<ListOptionItem<TValue>> AvailableValues { get; }

        /// <summary>
        /// Is fired when <see cref="AvailableValues" /> was changed.
        /// </summary>
        public event EventHandler<EventArgs>? AvailableValuesChanged;

        /// <summary>
        /// Raises the <see cref="AvailableValuesChanged" /> event.
        /// </summary>
        protected void RaiseAvailableValuesChanged()
        {
            AvailableValuesChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc />
        protected override bool ValidateValue(TValue value)
        {
            if (!base.ValidateValue(value))
                return false;

            return AvailableValues.Any(v => Equals(v.Value, value));
        }

        event EventHandler<EventArgs>? IListOption.AvailableValuesChanged
        {
            add => AvailableValuesChanged += value;
            remove => AvailableValuesChanged -= value;
        }

        IEnumerable<IListOptionItem> IListOption.AvailableValues => AvailableValues;

        object? IListOption.Value
        {
            get => Value;
            set => Value = value != null ? (TValue) value : Value;
        }
    }
}
