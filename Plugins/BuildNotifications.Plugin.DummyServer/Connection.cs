using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serialization;

namespace BuildNotifications.Plugin.DummyServer
{
    internal class Connection
    {
        public Connection(string url)
        {
            _restClient = new RestClient(url);
            _restClient.UseSerializer<JsonSerializer>();
        }

        public async Task<T> Get<T>(string path)
        {
            var request = new RestRequest(path, DataFormat.Json);
            return await _restClient.GetAsync<T>(request);
        }

        private readonly RestClient _restClient;
    }

    public class JsonSerializer : IRestSerializer
    {
        private readonly JsonSerializerSettings _settings;

        public JsonSerializer()
        {
            _settings = new JsonSerializerSettings();
            _settings.TypeNameHandling = TypeNameHandling.Objects;
        }

        public string Serialize(object obj) => JsonConvert.SerializeObject(obj, _settings);

        public string Serialize(Parameter parameter) => JsonConvert.SerializeObject(parameter.Value, _settings);

        public T Deserialize<T>(IRestResponse response) => JsonConvert.DeserializeObject<T>(response.Content, _settings);

        public string[] SupportedContentTypes { get; } =
        {
            "application/json", "text/json", "text/x-json", "text/javascript", "*+json"
        };

        public string ContentType { get; set; } = "application/json";

        public DataFormat DataFormat { get; } = DataFormat.Json;
    }
}