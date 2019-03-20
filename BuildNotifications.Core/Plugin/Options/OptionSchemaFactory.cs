using System.Collections.Generic;
using BuildNotifications.PluginInterfaces.Options;

namespace BuildNotifications.Core.Plugin.Options
{
    internal class OptionSchemaFactory : IOptionSchemaFactory
    {
        /// <inheritdoc />
        public IOption Flag(string name, string description, bool defaultValue = false, bool required = false)
        {
            return new FlagOption(name, description, defaultValue, required);
        }

        /// <inheritdoc />
        public IOptionGroup Group(string title, params IOption[] options)
        {
            var group = new OptionGroup(title);

            foreach( var option in options )
                group.Add(option);

            return group;
        }

        /// <inheritdoc />
        public INumberOption Number(string name, string description, int? minValue = null, int? maxValue = null, int defaultValue = 0, bool required = false)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public IOptionSchema Schema()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public ISetOption Set(string name, string description, IEnumerable<ISetItem> items, ISetItem defaultValue = null, bool required = false)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public ISetItem SetItem(string name, string description, object value)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public IOption Text(string name, string description, string defaultValue = null, bool required = false)
        {
            throw new System.NotImplementedException();
        }
    }
}