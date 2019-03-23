using BuildNotifications.PluginInterfaces.Options;

namespace BuildNotifications.Core.Plugin.Options
{
    internal class NumberOption : INumberOption
    {
        /// <inheritdoc />
        public NumberOption(string name, string? description, int? minValue, int? maxValue, int defaultValue, bool required)
        {
            Name = name;
            Description = description;
            MinValue = minValue;
            MaxValue = maxValue;
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
        public int DefaultValue { get; set; }

        /// <inheritdoc />
        public int? MinValue { get; set; }

        /// <inheritdoc />
        public int? MaxValue { get; set; }
    }
}