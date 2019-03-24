using System.Collections.Generic;

namespace BuildNotifications.Core.Config
{
    internal class Configuration : IConfiguration
    {
        public Configuration()
        {
            Connections = new List<IConnectionData>();
        }

        /// <inheritdoc />
        public IList<IConnectionData> Connections { get; set; }
    }
}