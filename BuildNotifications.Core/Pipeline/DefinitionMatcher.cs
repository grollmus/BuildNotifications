using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Core.Config;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Core.Pipeline;

internal class DefinitionMatcher : IBuildMatcher
{
    public DefinitionMatcher(IEnumerable<StringMatcher> matchers)
    {
        _matchers = matchers.ToList();
    }

    public bool IsMatch(IBaseBuild build)
    {
        return _matchers.Any(m => m.IsMatch(build.Definition.Name));
    }

    private readonly IEnumerable<StringMatcher> _matchers;
}