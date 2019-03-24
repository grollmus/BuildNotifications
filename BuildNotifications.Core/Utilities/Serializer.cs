using Newtonsoft.Json;

namespace BuildNotifications.Core.Utilities
{
    internal class Serializer : ISerializer
    {
        public Serializer()
        {
            _settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };
        }

        /// <inheritdoc />
        public string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value, Formatting.Indented, _settings);
        }

        /// <inheritdoc />
        public T Deserialize<T>(string serialized)
        {
            return JsonConvert.DeserializeObject<T>(serialized);
        }

        private readonly JsonSerializerSettings _settings;
    }
}