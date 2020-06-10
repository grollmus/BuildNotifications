using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace BuildNotifications.Core.Plugin
{
    internal class PluginAssemblyLoadContext : AssemblyLoadContext
    {
        public PluginAssemblyLoadContext(string folder)
        {
            _folder = folder;
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            try
            {
                return Default.LoadFromAssemblyName(assemblyName);
            }
            catch
            {
                // ignored
            }

            var fileName = assemblyName.Name + ".dll";
            var path = Path.Combine(_folder, fileName);
            if (File.Exists(path))
                return LoadFromAssemblyPath(path);

            return null;
        }

        private readonly string _folder;
    }
}