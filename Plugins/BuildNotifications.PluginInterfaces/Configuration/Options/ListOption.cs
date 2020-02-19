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
    public abstract class ListOption<TValue> : ValueOption<TValue>
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

        /// <inheritdoc />
        protected override bool ValidateValue(TValue value)
        {
            if (!base.ValidateValue(value))
                return false;

            return AvailableValues.Any(v => Equals(v.Value, value));
        }

        /// <summary>
        /// Is fired when <see cref="AvailableValues"/> was changed.
        /// </summary>
        public event EventHandler? AvailableValuesChanged;

        /// <summary>
        /// Raises the <see cref="AvailableValuesChanged"/> event.
        /// </summary>
        protected void RaiseAvailableValuesChanged()
        {
            AvailableValuesChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// </summary>
    [PublicAPI]
    public class ListOptionItem<TValue>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="value">Value of this item.</param>
        /// <param name="displayName">Text used for displaying this item.</param>
        public ListOptionItem(TValue value, string displayName)
        {
            Value = value;
            DisplayName = displayName;
        }

        /// <summary>
        /// Text used used for displaying this item
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Value of this item
        /// </summary>
        public TValue Value { get; }
    }
}