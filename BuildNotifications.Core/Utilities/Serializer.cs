using System;
using BuildNotifications.PluginInterfaces.Configuration;
using Newtonsoft.Json;

namespace BuildNotifications.Core.Utilities
{
    internal class Serializer : ISerializer
    {
        public Serializer()
        {
            _settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            _settings.Converters.Add(new PasswordStringConverter());
        }

        public string Serialize(object value) => JsonConvert.SerializeObject(value, Formatting.Indented, _settings);

        public T Deserialize<T>(string serialized) => JsonConvert.DeserializeObject<T>(serialized, _settings) ?? Activator.CreateInstance<T>()!;

        private readonly JsonSerializerSettings _settings;
    }
}