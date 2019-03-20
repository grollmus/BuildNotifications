using System;
using System.Collections.Generic;
using BuildNotifications.PluginInterfaces.Options;

namespace BuildNotifications.Core.Plugin.Options
{
    internal class OptionGroup : IOptionGroup
    {
        public OptionGroup(string title)
        {
            Title = title;
        }

        /// <inheritdoc />
        public string Title { get; set; }

        /// <inheritdoc />
        public void Add(IOption option)
        {
            if (option == null)
                throw new ArgumentNullException(nameof(option));

            _options.Add(option);
        }

        private readonly List<IOption> _options = new List<IOption>();
    }
}