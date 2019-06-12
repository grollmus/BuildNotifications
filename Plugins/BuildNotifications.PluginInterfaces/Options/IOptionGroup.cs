using System.Collections.Generic;
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
        /// Options in this group.
        /// </summary>
        IEnumerable<IOption> Options { get; }

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