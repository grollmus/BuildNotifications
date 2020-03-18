using System;
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

            CollectionTestOption = new StringCollectionOption(new[] {"One", "Two"}, "Collection Test", "This is a test for the collection option");
            CollectionDisplayOption = new DisplayOption(string.Empty, "Value of CollectionOption", "This is a test for the display option");

            CollectionTestOption.ValueChanged += CollectionTestOption_ValueChanged;
            RefreshDisplayOption();
        }

        public DisplayOption CollectionDisplayOption { get; }
        public StringCollectionOption CollectionTestOption { get; }
        public NumberOption Port { get; }
        public ICommandOption TestCommandOption { get; }

        public ConfigurationRawData AsRawData() => new ConfigurationRawData
        {
            Port = Port.Value
        };

        private bool CanExecuteTest() => Port.Value > 1000;

        private void CollectionTestOption_ValueChanged(object sender, EventArgs e)
        {
            RefreshDisplayOption();
        }

        private async Task ExecuteTest()
        {
            Debug.WriteLine("Starting test command");
            for (var i = 0; i < 3; i++)
            {
                await Task.Delay(1000);
                Debug.WriteLine("Testing...");
            }

            Debug.WriteLine("Done");
        }

        private void RefreshDisplayOption()
        {
            var value = string.Join(Environment.NewLine, CollectionTestOption.Value);
            CollectionDisplayOption.Value = value;
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
            yield return CollectionTestOption;
            yield return CollectionDisplayOption;
        }

        public string Serialize() => JsonConvert.SerializeObject(AsRawData());
    }

    public class DummyLocalizer : ILocalizer
    {
        public string Localized(string id, CultureInfo culture) => id;
    }

    public class ConfigurationRawData
    {
        public int Port { get; set; }
    }
}