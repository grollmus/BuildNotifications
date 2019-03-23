using BuildNotifications.PluginInterfaces.Options;

namespace BuildNotifications.Core.Plugin.Options
{
    internal class TextOption : ITextOption
    {
        public TextOption(string name, string? description, string? defaultValue, bool required)
        {
            Name = name;
            Description = description;
            DefaultValue = defaultValue;
            Required = required;
        }

        /// <inheritdoc />
        public string? Description { get; set; }

        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public bool Required { get; set; }

        /// <inheritdoc />
        public string? DefaultValue { get; set; }
    }
}