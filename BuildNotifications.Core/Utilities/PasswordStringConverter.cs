using System;
using BuildNotifications.PluginInterfaces;
using Newtonsoft.Json;
using NLog.Fluent;

namespace BuildNotifications.Core.Utilities
{
    internal class PasswordStringConverter : JsonConverter<PasswordString>
    {
        public override PasswordString ReadJson(JsonReader reader, Type objectType, PasswordString existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            try
            {
                var encrypted = (string) (reader.Value ?? string.Empty);
                if (string.IsNullOrEmpty(encrypted))
                    return new PasswordString(string.Empty);

                return new PasswordString(encrypted);
            }
            catch (Exception ex)
            {
                Log.Warn().Message("Failed to read encrypted password from configuration.").Exception(ex).Write();
                return new PasswordString(string.Empty);
            }
        }

        public override void WriteJson(JsonWriter writer, PasswordString value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Encrypted());
        }
    }
}