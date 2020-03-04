using System.Threading.Tasks;

namespace BuildNotifications.Core
{
    public static class TaskExtensions
    {
        /// <summary>
        /// Call this on a task to suppress compiler warning of not awaited call.
        /// </summary>
        /// <param name="_">Task to "ignore"</param>
        public static void FireAndForget(this Task _)
        {
            // Do nothing. Method is only used to get rid of compiler warning.
        }
    }
}