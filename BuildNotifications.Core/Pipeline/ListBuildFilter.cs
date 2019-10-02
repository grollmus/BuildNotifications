using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Utilities;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Core.Pipeline
{
    internal class ListBuildFilter : IBuildFilter
    {
        public ListBuildFilter(IProjectConfiguration projectConfiguration)
        {
            _projectConfiguration = projectConfiguration;
            InitializeStringMatcher();
        }

        public void InitializeStringMatcher()
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

            IBranchNameExtractor branchNameExtractor = new BranchNameExtractor();

            _allowingMatchers = new List<IBuildMatcher>
            {
                new DefinitionMatcher(Matchers(_projectConfiguration.BuildDefinitionWhitelist)),
                new BranchMatcher(branchNameExtractor, Matchers(_projectConfiguration.BranchWhitelist))
            };
            _forbiddingMatchers = new List<IBuildMatcher>
            {
                new DefinitionMatcher(Matchers(_projectConfiguration.BuildDefinitionBlacklist)),
                new BranchMatcher(branchNameExtractor, Matchers(_projectConfiguration.BranchBlacklist))
            };
        }

        public bool IsAllowed(IBaseBuild build)
        {
            if (_forbiddingMatchers.Any(m => m.IsMatch(build)))
                return _allowingMatchers.Any(m => m.IsMatch(build));

            return true;
        }

        private IEnumerable<StringMatcher> Matchers(IEnumerable<string> stringList)
        {
            return stringList.Select(x => new StringMatcher(x));
        }

        private readonly IProjectConfiguration _projectConfiguration;
        private List<IBuildMatcher> _allowingMatchers = new List<IBuildMatcher>();
        private List<IBuildMatcher> _forbiddingMatchers = new List<IBuildMatcher>();
        private string _lastLoadedLists = string.Empty;
    }
}