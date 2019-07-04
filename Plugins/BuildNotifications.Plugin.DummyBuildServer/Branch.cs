using System;
using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Plugin.DummyBuildServer
{
    internal class Branch : IBranch
    {
        public Branch()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public bool Equals(IBranch other)
        {
            return Name == (other as Branch)?.Name;
        }

        /// <inheritdoc />
        public string DisplayName { get; }

        /// <inheritdoc />
        public string Name { get; }
    }
}