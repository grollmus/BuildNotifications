using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Anotar.NLog;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Core.Plugin
{
    internal class PluginLoader : IPluginLoader
    {
        /// <inheritdoc />
        public IPluginRepository LoadPlugins(IEnumerable<string> folders)
        {
            var assemblies = folders.SelectMany(LoadPluginAssemblies);
            var exportedTypes = assemblies.SelectMany(a => a.GetExportedTypes())
                .Where(t => !t.IsAbstract)
                .ToList();

            var buildPlugins = LoadPluginsOfType<IBuildPlugin>(exportedTypes).ToList();
            var sourceControlPlugins = LoadPluginsOfType<ISourceControlPlugin>(exportedTypes).ToList();

            return new PluginRepository(buildPlugins, sourceControlPlugins);
        }

        private IEnumerable<Assembly> LoadPluginAssemblies(string folder)
        {
            var files = Directory.EnumerateDirectories(folder).SelectMany(dir => Directory.EnumerateFiles(dir, "*.dll"));

            foreach (var dll in files)
            {
                Assembly assembly;
                try
                {
                    assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(dll);
                }
                catch (Exception ex)
                {
                    LogTo.WarnException($"Exception while trying to load {dll}", ex);
                    continue;
                }

                yield return assembly;
            }
        }

        private IEnumerable<T> LoadPluginsOfType<T>(IEnumerable<Type> types)
        {
            var baseType = typeof(T);

            foreach (var type in types)
            {
                if (baseType.IsAssignableFrom(type))
                {
                    T value;
                    try
                    {
                        value = Activator.CreateInstance<T>();
                    }
                    catch (Exception ex)
                    {
                        var typeName = type.AssemblyQualifiedName;
                        LogTo.ErrorException($"Exception while trying to construct {typeName}", ex);
                        continue;
                    }

                    yield return value;
                }
            }
        }
    }
}