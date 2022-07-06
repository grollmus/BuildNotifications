using System;
using System.Windows.Markup;
using BuildNotifications.Core.Text;

namespace BuildNotifications.Resources.Text;

internal class Localized : MarkupExtension
{
    public Localized(string key)
    {
        Key = key;
    }

    public string Key { get; set; }

    public override object ProvideValue(IServiceProvider serviceProvider) => StringLocalizer.Instance[Key];
}