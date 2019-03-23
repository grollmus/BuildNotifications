using BuildNotifications.PluginInterfaces.Options;

namespace BuildNotifications.Core.Plugin.Options
{
    internal class SetItem : ISetItem
    {
        /// <inheritdoc />
        public SetItem(string name, string? description, object? value)
        {
            Name = name;
            Description = description;
            Value = value;
        }

        /// <inheritdoc />
        public object? Value { get; set; }

        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public string? Description { get; set; }
    }
}