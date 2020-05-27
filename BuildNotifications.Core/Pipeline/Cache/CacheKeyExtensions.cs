using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Core.Pipeline.Cache
{
    internal static class CacheKeyExtensions
    {
        public static CacheKey CacheKey(this IBuild build) => new CacheKey(build.ProjectId.ToString(), build.Id);
        
        public static CacheKey CacheKey(this IBranch branch, IProject project) => new CacheKey(project.Guid.ToString(), branch.FullName);
        
        public static CacheKey CacheKey(this IBuildDefinition definition, IProject project) => new CacheKey(project.Guid.ToString(), definition.Id);
    }
}
