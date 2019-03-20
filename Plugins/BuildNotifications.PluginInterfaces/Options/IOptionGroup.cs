using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Options
{
    /// <summary>
    /// Groups options together.
    /// </summary>
    [PublicAPI]
    public interface IOptionGroup
    {
        /// <summary>
        /// Title of the group.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Adds an option to this group.
        /// </summary>
        /// <param name="option">Option to add.</param>
        void Add(IOption option);
    }
}