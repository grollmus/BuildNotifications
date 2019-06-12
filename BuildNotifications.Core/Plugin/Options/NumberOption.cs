using BuildNotifications.PluginInterfaces.Options;

namespace BuildNotifications.Core.Plugin.Options
{
    internal class NumberOption : INumberOption
    {
        /// <inheritdoc />
        public NumberOption(string id, string name, string? description, int? minValue, int? maxValue, int defaultValue, bool required)
        {
            Id = id;
            Name = name;
            Description = description;
            MinValue = minValue;
            MaxValue = maxValue;
            DefaultValue = defaultValue;
            Value = DefaultValue;
            Required = required;
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
        public int DefaultValue { get; set; }

        /// <inheritdoc />
        public int? MinValue { get; set; }

        /// <inheritdoc />
        public int Value { get; set; }

        /// <inheritdoc />
        public int? MaxValue { get; set; }
    }
}