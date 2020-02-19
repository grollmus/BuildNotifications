using System.Collections.Generic;
using System.Globalization;
using BuildNotifications.PluginInterfaces.Configuration;
using BuildNotifications.PluginInterfaces.Configuration.Options;
using Newtonsoft.Json;

namespace BuildNotifications.Plugin.DummyBuildServer
{
    public class Configuration : IConfiguration
    {
        public Configuration()
        {
            Port = new NumberOption(1111, "Port", string.Empty)
            {
                MinValue = 1,
                MaxValue = 65536
            };
        }

        public NumberOption Port { get; }

        public ConfigurationRawData AsRawData()
        {
            return new ConfigurationRawData
            {
                Port = Port.Value
            };
        }

        public ILocalizer Localizer { get; } = new DummyLocalizer();

        public bool Deserialize(string serialized)
        {
            try
            {
                var raw = JsonConvert.DeserializeObject<ConfigurationRawData>(serialized);
                Port.Value = raw.Port;

                return true;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<IOption> ListAvailableOptions()
        {
            yield return Port;
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(AsRawData());
        }
    }

    public class DummyLocalizer : ILocalizer
    {
        public string Localized(string id, CultureInfo culture)
        {
            return id;
        }
    }

    public class ConfigurationRawData
    {
        public int Port { get; set; }
    }
}