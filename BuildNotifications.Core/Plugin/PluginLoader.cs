using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Anotar.NLog;
using BuildNotifications.Core.Utilities;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Core.Plugin
{
    internal class PluginAssemblyLoadContext : AssemblyLoadContext
    {
        public PluginAssemblyLoadContext(string folder)
        {
            _folder = folder;
        }

        /// <inheritdoc />
        protected override Assembly? Load(AssemblyName assemblyName)
        {
            try
            {
                var defaultAssembly = Default.LoadFromAssemblyName(assemblyName);
                if (defaultAssembly != null)
                    return defaultAssembly;
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

    internal class PluginLoader : IPluginLoader
    {
        private IEnumerable<Assembly> LoadPluginAssemblies(string folder)
        {
            var fullPath = Path.GetFullPath(folder);
            if (!Directory.Exists(fullPath))
                yield break;

            var pluginFolders = Directory.GetDirectories(fullPath);

            foreach (var pluginFolder in pluginFolders)
            {
                var assemblyLoadContext = new PluginAssemblyLoadContext(pluginFolder);

                var files = Directory.EnumerateFiles(pluginFolder, "*.dll");

                foreach (var dll in files)
                {
                    if (!Path.GetFileName(dll).Contains("plugin", StringComparison.OrdinalIgnoreCase))
                        continue;

                    Assembly assembly;
                    try
                    {
                        assembly = assemblyLoadContext.LoadFromAssemblyPath(dll);

                        foreach (var referencedAssembly in assembly.GetReferencedAssemblies())
                        {
                            assemblyLoadContext.LoadFromAssemblyName(referencedAssembly);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogTo.WarnException($"Exception while trying to load {dll}", ex);
                        continue;
                    }

                    yield return assembly;
                }
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
                        value = (T) Activator.CreateInstance(type);
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

        /// <inheritdoc />
        public IPluginRepository LoadPlugins(IEnumerable<string> folders)
        {
            var folderList = folders.ToList();

            var assemblies = folderList.SelectMany(LoadPluginAssemblies);
            var exportedTypes = assemblies.SelectMany(a => a.GetExportedTypes())
                .Where(t => !t.IsAbstract)
                .ToList();

            var buildPlugins = LoadPluginsOfType<IBuildPlugin>(exportedTypes).ToList();
            var sourceControlPlugins = LoadPluginsOfType<ISourceControlPlugin>(exportedTypes).ToList();

            return new PluginRepository(buildPlugins, sourceControlPlugins, new TypeMatcher());
        }
    }
}