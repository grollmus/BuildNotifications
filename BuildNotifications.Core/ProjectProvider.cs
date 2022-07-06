using System;
using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Pipeline;
using BuildNotifications.Core.Plugin;

namespace BuildNotifications.Core;

internal class ProjectProvider : IProjectProvider
{
    public ProjectProvider(IConfiguration configuration, IPluginRepository pluginRepository)
    {
        _configuration = configuration;

        _projectFactory = new ProjectFactory(pluginRepository, configuration);
        _projectFactory.ErrorOccured += (_, args) => ErrorOccured?.Invoke(this, args);
    }

    public event EventHandler<ErrorNotificationEventArgs>? ErrorOccured;

    private IEnumerable<IProject> Projects(Func<IProjectConfiguration, bool> predicate)
    {
        if (!_configuration.Projects.Any())
            yield break;

        foreach (var projectConfiguration in _configuration.Projects.Where(predicate))
        {
            var project = _projectFactory.Construct(projectConfiguration);
            if (project != null)
                yield return project;
        }
    }

    public IEnumerable<IProject> AllProjects() => Projects(_ => true);

    public IEnumerable<IProject> EnabledProjects() => Projects(x => x.IsEnabled);

    private readonly IConfiguration _configuration;
    private readonly ProjectFactory _projectFactory;
}