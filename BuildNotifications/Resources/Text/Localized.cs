using System;
using System.Windows.Markup;
using BuildNotifications.Core.Text;

namespace BuildNotifications.Resources.Text
{
    internal class Localized : MarkupExtension
    {
        public string Key { get; set; }

        public Localized(string key)
        {
            Key = key;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return StringLocalizer.Instance[Key];
        }
    }
}