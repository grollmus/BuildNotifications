using BuildNotifications.PluginInterfaces.Options;

namespace BuildNotifications.Core.Plugin.Options
{
    internal class FlagOption : IFlagOption
    {
        public FlagOption(string id, string name, string? description, bool defaultValue, bool required)
        {
            Id = id;
            Name = name;
            Description = description;
            DefaultValue = defaultValue;
            Required = required;
        }

        /// <inheritdoc />
        public bool DefaultValue { get; set; }

        /// <inheritdoc />
        public bool Value { get; set; }

        /// <inheritdoc />
        public string? Description { get; set; }

        /// <inheritdoc />
        public string Id { get; set; }

        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public bool Required { get; set; }
    }
}