using System;
using System.Collections.Generic;
using System.Linq;
using BuildNotifications.PluginInterfaces.Options;

namespace BuildNotifications.Core.Plugin.Options
{
    internal class OptionSchemaFactory : IOptionSchemaFactory
    {
        /// <inheritdoc />
        public IFlagOption Flag(string id, string name, string? description, bool defaultValue = false, bool required = false)
        {
            return new FlagOption(id, name, description, defaultValue, required);
        }

        /// <inheritdoc />
        public IOptionGroup Group(string title, params IOption[] options)
        {
            var group = new OptionGroup(title);

            foreach (var option in options)
            {
                group.Add(option);
            }

            return group;
        }

        /// <inheritdoc />
        public INumberOption Number(string id, string name, string? description, int? minValue = null, int? maxValue = null,
            int defaultValue = 0, bool required = false)
        {
            if (minValue.HasValue && maxValue.HasValue && minValue >= maxValue)
            {
                throw new ArgumentException($"{nameof(minValue)} must be smaller than {nameof(maxValue)}");
            }

            return new NumberOption(id, name, description, minValue, maxValue, defaultValue, required);
        }

        /// <inheritdoc />
        public IOptionSchema Schema()
        {
            return new OptionSchema();
        }

        /// <inheritdoc />
        public ISetOption Set(string id, string name, string? description, IEnumerable<ISetItem> items, ISetItem? defaultValue = null,
            bool required = false)
        {
            var itemList = items.ToArray();
            if (defaultValue != null && !itemList.Contains(defaultValue))
            {
                throw new ArgumentException($"{nameof(defaultValue)} is not contained in {nameof(items)}");
            }

            return new SetOption(id, name, description, itemList, defaultValue, required);
        }

        /// <inheritdoc />
        public ISetItem SetItem(string name, string? description, object? value)
        {
            return new SetItem(name, description, value);
        }

        /// <inheritdoc />
        public ITextOption Text(string id, string name, string? description, string? defaultValue = null, bool required = false)
        {
            return new TextOption(id, name, description, defaultValue, required);
        }
    }
}