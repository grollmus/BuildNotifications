using System.Collections.Generic;
using BuildNotifications.PluginInterfaces.Options;

namespace BuildNotifications.Core.Plugin.Options
{
    internal class OptionSchema : IOptionSchema
    {
        /// <inheritdoc />
        public IEnumerable<IOption> GlobalOptions => _globalOptions;

        /// <inheritdoc />
        public IEnumerable<IOptionGroup> Groups => _groups;

        /// <inheritdoc />
        public void Add(IOptionGroup group)
        {
            _groups.Add(group);
        }

        /// <inheritdoc />
        public void Add(IOption option)
        {
            _globalOptions.Add(option);
        }

        private readonly List<IOptionGroup> _groups = new List<IOptionGroup>();
        private readonly List<IOption> _globalOptions = new List<IOption>();
    }
}