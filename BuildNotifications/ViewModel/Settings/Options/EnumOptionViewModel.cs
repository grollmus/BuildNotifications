using System;
using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Core.Text;

namespace BuildNotifications.ViewModel.Settings.Options
{
    public class EnumOptionViewModel<TEnum> : ListOptionBaseViewModel<TEnum>
        where TEnum : struct
    {
        public EnumOptionViewModel(string displayName, TEnum value = default)
            : base(displayName, value)
        {
        }

        protected override IEnumerable<TEnum> ModelValues => Enum.GetValues(typeof(TEnum)).Cast<TEnum>();

        protected override string DisplayNameFor(TEnum item) => StringLocalizer.Instance.GetText(item.ToString() ?? string.Empty) ?? item.ToString()!;
    }
}