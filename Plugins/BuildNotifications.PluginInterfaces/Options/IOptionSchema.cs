using System.Collections.Generic;
using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Options
{
    /// <summary>
    /// Schema describing a set of options.
    /// </summary>
    [PublicAPI]
    public interface IOptionSchema
    {
        /// <summary>
        /// Options in this schema that are not part of a group.
        /// </summary>
        IEnumerable<IOption> GlobalOptions { get; }

        /// <summary>
        /// Groups in this schema.
        /// </summary>
        IEnumerable<IOptionGroup> Groups { get; }

        /// <summary>
        /// Adds a group to this schema.
        /// </summary>
        /// <param name="group">Group to add.</param>
        void Add(IOptionGroup group);

        /// <summary>
        /// Adds an option to this schema.
        /// </summary>
        /// <param name="option">Option to add.</param>
        void Add(IOption option);
    }
}