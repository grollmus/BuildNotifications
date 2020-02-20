using System;
using System.Collections.Generic;
using System.Linq;

namespace BuildNotifications.ViewModel.Settings.Options
{
    public abstract class EnumOptionBaseViewModel<TEnum> : ListOptionBaseViewModel<TEnum>
        where TEnum : struct
    {
        protected EnumOptionBaseViewModel(string displayName, TEnum value = default)
            : base(displayName, value)
        {
        }

        protected override IEnumerable<TEnum> ModelValues => Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
    }
}