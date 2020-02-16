using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Core.Config;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Core.Pipeline
{
    internal class ListBuildFilter : IBuildFilter
    {
        public ListBuildFilter(IProjectConfiguration projectConfiguration, IBranchNameExtractor branchNameExtractor)
        {
            _projectConfiguration = projectConfiguration;
            InitializeStringMatcher(branchNameExtractor);
        }

        public void InitializeStringMatcher(IBranchNameExtractor nameExtractor)
        {
            var list = string.Join('.', new[]
            {
                _projectConfiguration.BranchBlacklist,
                _projectConfiguration.BranchWhitelist,
                _projectConfiguration.BuildDefinitionBlacklist,
                _projectConfiguration.BuildDefinitionWhitelist
            }.Flatten());

            if (_lastLoadedLists == list)
                return;

            _lastLoadedLists = list;

            _allowingMatchers = new List<IBuildMatcher>
            {
                new DefinitionMatcher(Matchers(_projectConfiguration.BuildDefinitionWhitelist)),
                new BranchMatcher(nameExtractor, Matchers(_projectConfiguration.BranchWhitelist))
            };
            _forbiddingMatchers = new List<IBuildMatcher>
            {
                new DefinitionMatcher(Matchers(_projectConfiguration.BuildDefinitionBlacklist)),
                new BranchMatcher(nameExtractor, Matchers(_projectConfiguration.BranchBlacklist))
            };
        }

        private IEnumerable<StringMatcher> Matchers(IEnumerable<string> stringList)
        {
            return stringList.Select(x => new StringMatcher(x));
        }

        public bool IsAllowed(IBaseBuild build)
        {
            if (_forbiddingMatchers.Any(m => m.IsMatch(build)))
                return _allowingMatchers.Any(m => m.IsMatch(build));

            return true;
        }

        private readonly IProjectConfiguration _projectConfiguration;
        private List<IBuildMatcher> _allowingMatchers = new List<IBuildMatcher>();
        private List<IBuildMatcher> _forbiddingMatchers = new List<IBuildMatcher>();
        private string _lastLoadedLists = string.Empty;
    }
}