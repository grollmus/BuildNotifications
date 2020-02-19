using System;
using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Configuration
{
    /// <summary>
    /// Properties for the ValueChanged event of an Option.
    /// </summary>
    [PublicAPI]
    public class ValueChangedEventArgs<TValue> : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        public ValueChangedEventArgs(TValue oldValue, TValue newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        /// <summary>
        /// The new value of the option.
        /// </summary>
        public TValue NewValue { get; }

        /// <summary>
        /// The previous value of the option.
        /// </summary>
        public TValue OldValue { get; }
    }
}