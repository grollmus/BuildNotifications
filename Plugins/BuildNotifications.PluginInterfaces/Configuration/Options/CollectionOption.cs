using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Configuration.Options;

/// <summary>
/// Base for an option that contains a collection of items.
/// </summary>
/// <typeparam name="TItem">Type of item contained in this option.</typeparam>
[PublicAPI]
public class CollectionOption<TItem> : ValueOption<List<TItem>>
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="value">Initial value of this option.</param>
    /// <param name="nameTextId">Text id used for localizing the name of this option.</param>
    /// <param name="descriptionTextId">Text id used for localizing the description of this option.</param>
    protected CollectionOption(IEnumerable<TItem> value, string nameTextId, string descriptionTextId)
        : base(value.ToList(), nameTextId, descriptionTextId)
    {
    }

    /// <summary>
    /// Adds a new empty item to the collection.
    /// </summary>
    public void AddNewItem(TItem item)
    {
        Value.Add(item);
        RaiseValueChanged();
    }

    /// <summary>
    /// Removes an item from the collection.
    /// </summary>
    /// <param name="item">Item to remove</param>
    public void RemoveItem(TItem item)
    {
        Value.Remove(item);
        RaiseValueChanged();
    }
}