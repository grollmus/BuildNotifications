using System.Collections.Generic;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Core.Pipeline.Notification;

internal class DefinitionNotification : BaseBuildNotification
{
    public DefinitionNotification(IList<IBuildNode> buildNodes, BuildStatus status, IList<string> definitionNames)
        : base(NotificationType.Definition, buildNodes, status)
    {
        _definitionNames = definitionNames;
        SetParameters();
    }

    protected override string GetMessageTextId()
    {
        return _definitionNames.Count switch
        {
            1 => DefinitionChangedTextId,
            2 => TwoDefinitionsChangedTextId,
            _ => ThreeDefinitionsChangedTextId
        };
    }

    protected override string ResolveIssueSource() => string.Join("\n", _definitionNames);

    private void SetParameters()
    {
        Parameters.Clear();
        Parameters.Add(StatusTextId(_definitionNames.Count == 1));
        Parameters.AddRange(_definitionNames);
    }

    private readonly IList<string> _definitionNames;

    // Definition {1} {0}. E.g. Definition CI failed.
    public const string DefinitionChangedTextId = nameof(DefinitionChangedTextId);

    // Definitions {1}, {2} and {3} {0}. E.g. Definition CI, Nightly and Cloud failed.
    public const string ThreeDefinitionsChangedTextId = nameof(ThreeDefinitionsChangedTextId);

    // Definitions {1} and {2} {0}. E.g. Definition CI and Nightly failed.
    public const string TwoDefinitionsChangedTextId = nameof(TwoDefinitionsChangedTextId);
}