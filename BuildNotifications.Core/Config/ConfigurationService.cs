using System;

namespace BuildNotifications.Core.Config
{
    internal class ConfigurationService : IConfigurationService
    {
        public ConfigurationService(IConfigurationSerializer serializer, IConfigurationBuilder configurationBuilder)
        {
            Serializer = serializer;
            CurrentConfig = configurationBuilder.LoadConfiguration();
        }

        public IConfigurationSerializer Serializer { get; }
        public IConfiguration CurrentConfig { get; }

        public void Merge(IConfiguration newConfiguration)
        {
            throw new NotImplementedException();
        }
    }
}