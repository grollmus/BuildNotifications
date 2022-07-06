using System.Collections.Generic;
using System.Globalization;
using BuildNotifications.PluginInterfaces.Configuration;
using BuildNotifications.PluginInterfaces.Configuration.Options;
using Newtonsoft.Json;

namespace BuildNotifications.Plugin.DummyServer;

public class Configuration : IPluginConfiguration
{
    public Configuration()
    {
        Url = new TextOption("https://localhost:44311/", "Url", string.Empty);
    }

    public TextOption Url { get; }

    public ConfigurationRawData AsRawData() => new()
    {
        Url = Url.Value
    };

    public ILocalizer Localizer { get; } = new DummyLocalizer();

    public bool Deserialize(string serialized)
    {
        try
        {
            var raw = JsonConvert.DeserializeObject<ConfigurationRawData>(serialized);
            if (raw == null)
                return false;

            Url.Value = raw.Url;

            return true;
        }
        catch
        {
            return false;
        }
    }

    public IEnumerable<IOption> ListAvailableOptions()
    {
        yield return Url;
    }

    public string Serialize() => JsonConvert.SerializeObject(AsRawData());
}

public class DummyLocalizer : ILocalizer
{
    public string Localized(string id, CultureInfo culture) => id;
}

public class ConfigurationRawData
{
    public string Url { get; set; }
}