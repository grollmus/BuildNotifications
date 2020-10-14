using System;
using System.Threading;
using System.Threading.Tasks;

namespace BuildNotifications.Core.Toolkit
{
    public class PluginTestActionFactory
    {
        public IPluginTestAction CreateTest(Func<CancellationToken, Task> taskCreationFunction, string name)
        {
            return new PluginTestAction(taskCreationFunction, name);
        }
    }
}