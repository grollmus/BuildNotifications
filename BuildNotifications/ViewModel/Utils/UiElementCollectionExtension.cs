using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace BuildNotifications.ViewModel.Utils;

/// <summary>
/// Makes it easier to iterate over a UiElementCollection
/// </summary>
internal static class UiElementCollectionExtension
{
    public static IEnumerable<UIElement> Enumerate(this UIElementCollection collection)
    {
        for (var index = 0; index < collection.Count; index++)
        {
            var element = collection[index];
            yield return element;
        }
    }
}