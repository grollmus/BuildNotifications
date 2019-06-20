using BuildNotifications.PluginInterfaces.Options;

namespace BuildNotifications.Core.Plugin.Options
{
    internal class SetOption : ISetOption
    {
        public SetOption(string id, string name, string? description, ISetItem[] values, ISetItem? defaultValue, in bool required)
        {
            Id = id;
            Name = name;
            Description = description;
            Values = values;
            DefaultValue = defaultValue;
            Required = required;
            SelectedValue = DefaultValue;
        }

        /// <inheritdoc />
        public string? Description { get; set; }

        /// <inheritdoc />
        public string Id { get; set; }

        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public bool Required { get; set; }

        /// <inheritdoc />
        public ISetItem? SelectedValue { get; set; }

        /// <inheritdoc />
        public ISetItem[] Values { get; set; }

        /// <inheritdoc />
        public ISetItem? DefaultValue { get; set; }
    }
}