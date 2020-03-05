using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using BuildNotifications.PluginInterfaces.Configuration;
using BuildNotifications.PluginInterfaces.Configuration.Options;
using Newtonsoft.Json;

namespace BuildNotifications.Plugin.DummyBuildServer
{
    public class Configuration : IPluginConfiguration
    {
        public Configuration()
        {
            Port = new NumberOption(1111, "Port", string.Empty)
            {
                MinValue = 1,
                MaxValue = 65536
            };

            TestCommandOption = new CommandOption(ExecuteTest, CanExecuteTest, "Test", "Performs a test");
        }

        private async Task ExecuteTest()
        {
            Debug.WriteLine("Starting test command");
            for (int i = 0; i < 3; i++)
            {
                await Task.Delay(1000);
                Debug.WriteLine("Testing...");
            }

            Debug.WriteLine("Done");
        }

        private bool CanExecuteTest()
        {
            return Port.Value > 1000;
        }

        public NumberOption Port { get; }
        public ICommandOption TestCommandOption { get; }

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
            yield return TestCommandOption;
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