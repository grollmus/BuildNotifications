using System;
using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Configuration.Options
{
    /// <summary>
    /// Option containing an integer
    /// </summary>
    [PublicAPI]
    public class NumberOption : ValueOption<int>
    {
        /// <inheritdoc cref="ValueOption{TValue}" />
        public NumberOption(int value, string nameTextId, string descriptionTextId)
            : base(value, nameTextId, descriptionTextId)
        {
        }

        /// <summary>
        /// Maximum allowed value of this option.
        /// </summary>
        public int MaxValue { get; set; } = int.MaxValue;

        /// <summary>
        /// Minimum allowed value of this option.
        /// </summary>
        public int MinValue { get; set; } = int.MinValue;

        /// <inheritdoc />
        protected override int Clamp(int value) => Math.Max(MinValue, Math.Min(MaxValue, value));
    }
}