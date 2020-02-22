using System;
using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Configuration.Options
{
    /// <summary>
    /// Base class for an option.
    /// </summary>
    [PublicAPI]
    public abstract class Option : IOption
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="nameTextId">Text id used for localizing the name of this option.</param>
        /// <param name="descriptionTextId">Text id used for localizing the description of this option.</param>
        protected Option(string nameTextId, string descriptionTextId)
        {
            NameTextId = nameTextId;
            DescriptionTextId = descriptionTextId;
        }

        /// <summary>
        /// Raises the <see cref="IsEnabledChanged" /> event.
        /// </summary>
        protected void RaiseIsEnabledChanged()
        {
            IsEnabledChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raises the <see cref="IsVisibleChanged" /> event.
        /// </summary>
        protected void RaiseIsVisibleChanged()
        {
            IsVisibleChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc />
        public bool IsLoading { get; protected set; }

        /// <inheritdoc />
        public string NameTextId { get; }

        /// <inheritdoc />
        public event EventHandler? IsEnabledChanged;

        /// <inheritdoc />
        public event EventHandler? IsVisibleChanged;

        /// <inheritdoc />
        public string DescriptionTextId { get; }

        /// <summary>
        /// Determine whether this option should be editable by the user.
        /// </summary>
        public virtual bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled == value)
                    return;

                _isEnabled = value;
                RaiseIsEnabledChanged();
            }
        }

        /// <summary>
        /// Determine whether this option should be visible in the UI.
        /// </summary>
        public virtual bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible == value)
                    return;

                _isVisible = value;
                RaiseIsVisibleChanged();
            }
        }

        private bool _isEnabled = true;
        private bool _isVisible = true;
    }
}