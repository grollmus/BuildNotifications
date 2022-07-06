using System.Collections.Generic;
using BuildNotifications.Core.Pipeline;

namespace BuildNotifications.Core;

public interface IProjectProvider
{
    IEnumerable<IProject> AllProjects();

    IEnumerable<IProject> EnabledProjects();
}