using System.Collections.Generic;
using System.IO;
using BuildNotifications.Core;

namespace BuildNotifications
{
    internal class PathResolver : IPathResolver
    {
        public string ConfigurationFolder
        {
            get
            {
#if DEBUG
                return ".";
#else
                return Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "BuildNotifications", "data");
#endif
            }
        }

        public string UserConfigurationFileName => "config.json";
        public string UserConfigurationFilePath => Path.Combine(ConfigurationFolder, UserConfigurationFileName);

        public IEnumerable<string> PluginFolders
        {
            get { yield return "plugins"; }
        }

        public string PredefinedConfigurationFileName => "predefined.json";
        public string PredefinedConfigurationFilePath => Path.Combine(ConfigurationFolder, PredefinedConfigurationFileName);
    }
}