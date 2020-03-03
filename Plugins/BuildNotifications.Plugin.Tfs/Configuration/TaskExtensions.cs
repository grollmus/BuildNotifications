using System.Threading.Tasks;

namespace BuildNotifications.Plugin.Tfs.Configuration
{
    internal static class TaskExtensions
    {
        /// <summary>
        /// Call this on a task to suppress compiler warning of not awaited call.
        /// </summary>
        /// <param name="task">Task to "ignore"</param>
#pragma warning disable IDE0060 // Remove unused parameter
        public static void FireAndForget(this Task task)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            // Do nothing. Method is only used to get rid of compiler warning.
        }
    }
}