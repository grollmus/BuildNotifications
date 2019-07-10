using System.IO;
using Newtonsoft.Json;

namespace DummyBuildServer.Models
{
    internal class DataSerializer
    {
        public ServerConfig Load(string fileName)
        {
            if (!File.Exists(fileName))
                return new ServerConfig();

            var json = File.ReadAllText(fileName);
            return JsonConvert.DeserializeObject<ServerConfig>(json);
        }

        public void Save(ServerConfig config, string fileName)
        {
            var json = JsonConvert.SerializeObject(config);
            File.WriteAllText(fileName, json);
        }
    }
}