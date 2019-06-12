using BuildNotifications.PluginInterfaces.Options;

namespace BuildNotifications.Core.Plugin.Options
{
    internal class TextOption : ITextOption
    {
        public TextOption(string id, string name, string? description, string? defaultValue, bool required)
        {
            Id = id;
            Name = name;
            Description = description;
            DefaultValue = defaultValue;
            Required = required;
            Value = DefaultValue ?? string.Empty;
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
        public string? DefaultValue { get; set; }

        /// <inheritdoc />
        public string Value { get; set; }
    }
}