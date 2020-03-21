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
        public ProjectOption()
            : base(null, TextIds.ProjectName, TextIds.ProjectDescription)
        {
        }

        public override IEnumerable<ListOptionItem<TfsProject?>> AvailableValues
        {
            get { return _availableProjects.Select(p => new ListOptionItem<TfsProject?>(p, p.ProjectName ?? string.Empty)); }
        }

        public async Task<IEnumerable<TfsProject>> FetchAvailableProjects(TfsConfigurationRawData rawData)
        {
            try
            {
                if (string.IsNullOrEmpty(rawData.Url) || string.IsNullOrEmpty(rawData.CollectionName))
                    return Enumerable.Empty<TfsProject>();
                if (rawData.AuthenticationType == AuthenticationType.Token && string.IsNullOrEmpty(rawData.Token?.PlainText()))
                    return Enumerable.Empty<TfsProject>();
                if (rawData.AuthenticationType == AuthenticationType.Account && (string.IsNullOrEmpty(rawData.Username) || string.IsNullOrEmpty(rawData.Password?.PlainText())))
                    return Enumerable.Empty<TfsProject>();

                var pool = new TfsConnectionPool();
                var vssConnection = pool.CreateConnection(rawData);
                if (vssConnection == null)
                {
                    _availableProjects.Clear();
                    return Enumerable.Empty<TfsProject>();
                }

                var projectClient = vssConnection.GetClient<ProjectHttpClient>();
                var projects = await projectClient.GetProjects(ProjectState.WellFormed);

                return projects.Select(p => new TfsProject(p));
            }
            catch (Exception e)
            {
                LogTo.InfoException("Failed to fetch projects", e);
                return Enumerable.Empty<TfsProject>();
            }
        }

        public void SetAvailableProjects(IEnumerable<TfsProject> availableProjects)
        {
            _availableProjects = availableProjects.ToList();
            RaiseAvailableValuesChanged();
            Value = _availableProjects.FirstOrDefault(r => Equals(r, Value));
        }

        protected override bool ValidateValue(TfsProject? value)
        {
            return value != null && !string.IsNullOrEmpty(value.Id);
        }

        private List<TfsProject> _availableProjects = new List<TfsProject>();
    }
}