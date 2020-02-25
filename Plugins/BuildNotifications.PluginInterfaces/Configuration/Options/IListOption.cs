using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Configuration.Options
{
    /// <summary>
    /// </summary>
    [PublicAPI]
    public interface IListOption : IOption
    {
        /// <summary>
        /// List of all available values for this option.
        /// </summary>
        IEnumerable<IListOptionItem> AvailableValues { get; }

        /// <summary>
        /// Current value of this option.
        /// </summary>
        object? Value { get; set; }

        /// <summary>
        /// Is fired when <see cref="IListOption.AvailableValues" /> was changed.
        /// </summary>
        event EventHandler? AvailableValuesChanged;
    }
}