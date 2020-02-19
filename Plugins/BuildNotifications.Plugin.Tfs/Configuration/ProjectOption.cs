using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anotar.NLog;
using BuildNotifications.PluginInterfaces.Configuration.Options;
using Microsoft.TeamFoundation.Core.WebApi;

namespace BuildNotifications.Plugin.Tfs.Configuration
{
    internal class ProjectOption : ListOption<TfsProject?>
    {
        public ProjectOption() : base(null, TextIds.ProjectName, TextIds.ProjectDescription)
        {
        }

        public override IEnumerable<ListOptionItem<TfsProject?>> AvailableValues
        {
            get { return _availableProjects.Select(p => new ListOptionItem<TfsProject?>(p, p.ProjectName ?? string.Empty)); }
        }

        public async Task FetchAvailableProjects(TfsConfigurationRawData rawData)
        {
            try
            {
                var pool = new TfsConnectionPool();
                var vssConnection = pool.CreateConnection(rawData);
                if (vssConnection == null)
                {
                    _availableProjects.Clear();
                    return;
                }

                var projectClient = vssConnection.GetClient<ProjectHttpClient>();
                var projects = await projectClient.GetProjects(ProjectState.WellFormed);

                _availableProjects = projects.Select(p => new TfsProject(p)).ToList();
            }
            catch (Exception e)
            {
                LogTo.InfoException("Failed to fetch projects", e);
            }
            finally
            {
                RaiseAvailableValuesChanged();
            }
        }

        private List<TfsProject> _availableProjects = new List<TfsProject>();
    }
}