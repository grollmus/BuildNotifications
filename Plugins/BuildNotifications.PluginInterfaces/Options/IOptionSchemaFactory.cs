using System.Collections.Generic;
using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Options
{
    /// <summary>
    /// Factory that can be used to construct elements used by an OptionSchema.
    /// </summary>
    [PublicAPI]
    public interface IOptionSchemaFactory
    {
        /// <summary>
        /// Constructs a new FlagOption.
        /// </summary>
        /// <param name="name">Name of the option</param>
        /// <param name="description">Detailed description of the option.</param>
        /// <param name="defaultValue">Default value for the option.</param>
        /// <param name="required">Flag indicating whether the option is required.</param>
        /// <returns>The created option.</returns>
        IOption Flag(string name, string description, bool defaultValue = false, bool required = false);

        /// <summary>
        /// Constructs a new OptionGroup with a list of options.
        /// </summary>
        /// <param name="title">Title of the group.</param>
        /// <param name="options">Options the group will be filled with initially.</param>
        /// <returns>The created group.</returns>
        IOptionGroup Group(string title, params IOption[] options);

        /// <summary>
        /// Constructs a new NumberOption.
        /// </summary>
        /// <param name="name">Name of the option</param>
        /// <param name="description">Detailed description of the option.</param>
        /// <param name="minValue">Minimum allowed value.</param>
        /// <param name="maxValue">Maximum allowed value.</param>
        /// <param name="defaultValue">Default value for the option.</param>
        /// <param name="required">Flag indicating whether the option is required.</param>
        /// <returns></returns>
        INumberOption Number(string name, string description, int? minValue = null, int? maxValue = null, int defaultValue = 0, bool required = false);

        /// <summary>
        /// Constructs a new schema.
        /// </summary>
        /// <returns>The constructed schema.</returns>
        IOptionSchema Schema();

        /// <summary>
        /// Constructs a new SetOption.
        /// </summary>
        /// <param name="name">Name of the option</param>
        /// <param name="description">Detailed description of the option.</param>
        /// <param name="items">Items that should be contained in the set.</param>
        /// <param name="defaultValue">Default value for the option.</param>
        /// <param name="required">Flag indicating whether the option is required.</param>
        /// <returns></returns>
        ISetOption Set(string name, string description, IEnumerable<ISetItem> items, ISetItem defaultValue = null, bool required = false);

        /// <summary>
        /// Constructs a SetItem that can be used in a SetOption.
        /// </summary>
        /// <param name="name">Name of the item.</param>
        /// <param name="description">Description of the item.</param>
        /// <param name="value">Value of the item.</param>
        /// <returns>The created item.</returns>
        ISetItem SetItem(string name, string description, object value);

        /// <summary>
        /// Constructs a new TextOption.
        /// </summary>
        /// <param name="name">Name of the option</param>
        /// <param name="description">Detailed description of the option.</param>
        /// <param name="defaultValue">Default value for the option.</param>
        /// <param name="required">Flag indicating whether the option is required.</param>
        /// <returns>The created option.</returns>
        IOption Text(string name, string description, string defaultValue = null, bool required = false);
    }
}