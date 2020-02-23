using System;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace BuildNotifications.PluginInterfaces.Configuration
{
    /// <summary>
    /// Converter that can be used to read and write <see cref="PasswordString" />
    /// using Newtonsft.Json.
    /// </summary>
    [PublicAPI]
    public class PasswordStringConverter : JsonConverter<PasswordString>
    {
        /// <inheritdoc />
        public override PasswordString ReadJson(JsonReader reader, Type objectType, PasswordString existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            try
            {
                var encrypted = (string) (reader.Value ?? string.Empty);
                if (string.IsNullOrEmpty(encrypted))
                    return new PasswordString(string.Empty);

                return new PasswordString(encrypted);
            }
            catch
            {
                return new PasswordString(string.Empty);
            }
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, PasswordString value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Encrypted());
        }
    }
}