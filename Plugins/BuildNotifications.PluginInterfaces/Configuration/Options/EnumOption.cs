using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Configuration.Options
{
    /// <summary>
    /// Option containing an Enum.
    /// </summary>
    /// <typeparam name="TEnum">Enum type that is used for this option.</typeparam>
    [PublicAPI]
    public class EnumOption<TEnum> : ListOption<TEnum>
        where TEnum : struct
    {
        /// <inheritdoc />
        public EnumOption(TEnum value, string nameTextId, string descriptionTextId)
            : base(value, nameTextId, descriptionTextId)
        {
        }

        /// <summary>
        /// List of all available values for this option. Default is every value of the enumeration.
        /// </summary>
        public override IEnumerable<ListOptionItem<TEnum>> AvailableValues
        {
            get
            {
                var enumType = typeof(TEnum);
                return Enum.GetValues(enumType).Cast<TEnum>().Select(e =>
                {
                    var textId = $"{enumType.FullName}.{e}";
                    return new ListOptionItem<TEnum>(e, textId);
                });
            }
        }

        /// <inheritdoc />
        protected override bool ValidateValue(TEnum value)
        {
            if (!base.ValidateValue(value))
                return false;

            return AvailableValues.Any(v => Equals(v.Value, value));
        }
    }
}