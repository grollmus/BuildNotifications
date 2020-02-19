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

        /// <inheritdoc />
        public bool IsLoading { get; protected set; }

        /// <inheritdoc />
        public string NameTextId { get; }

        /// <inheritdoc />
        public string DescriptionTextId { get; }

        /// <summary>
        /// Determine whether this option should be editable by the user.
        /// </summary>
        public virtual bool IsEnabled { get; set; }

        /// <summary>
        /// Determine whether this option should be visible in the UI.
        /// </summary>
        public virtual bool IsVisible { get; set; }
    }
}