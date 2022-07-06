using System.Collections.Generic;
using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Configuration.Options;

/// <summary>
/// Option that contains a collection of strings.
/// </summary>
[PublicAPI]
public class StringCollectionOption : CollectionOption<string>
{
    /// <inheritdoc />
    public StringCollectionOption(IEnumerable<string> value, string nameTextId, string descriptionTextId)
        : base(value, nameTextId, descriptionTextId)
    {
    }
}