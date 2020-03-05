using System;
using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Configuration.Options
{
    /// <summary>
    /// A configuration option that is exposed to the application
    /// </summary>
    [PublicAPI]
    public interface IOption
    {
        /// <summary>
        /// Text id used for localizing the description of this option.
        /// </summary>
        string DescriptionTextId { get; }

        /// <summary>
        /// Determine whether this option should be editable by the user.
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Indicates whether this option is currently calculation available values.
        /// </summary>
        bool IsLoading { get; }

        /// <summary>
        /// Determine whether this option should be visible in the UI.
        /// </summary>
        bool IsVisible { get; set; }

        /// <summary>
        /// Text id used for localizing the name of this option.
        /// </summary>
        string NameTextId { get; }

        /// <summary>
        /// Occurs when the value of the <see cref="IsEnabled" /> property changed.
        /// </summary>
        event EventHandler<EventArgs>? IsEnabledChanged;

        /// <summary>
        /// Occurs when the value of the <see cref="IsVisible" /> property changed.
        /// </summary>
        event EventHandler<EventArgs>? IsVisibleChanged;

        /// <summary>
        /// Occurs when the value of the <see cref="IsLoading" /> property changed.
        /// </summary>
        event EventHandler<EventArgs>? IsLoadingChanged;
    }
}