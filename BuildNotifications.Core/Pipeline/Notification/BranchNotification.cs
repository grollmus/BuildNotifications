using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Core.Pipeline.Notification;

internal class BranchNotification : BaseBuildNotification
{
    public BranchNotification(IList<IBuildNode> buildNodes, BuildStatus status, IEnumerable<string> branchNames)
        : base(NotificationType.Branch, buildNodes, status)
    {
        _branchNames = branchNames.Distinct().ToList();
        SetParameter();
    }

    protected override string GetMessageTextId()
    {
        return _branchNames.Count switch
        {
            1 => BranchChangedTextId,
            2 => TwoBranchesChangedTextId,
            _ => ThreeBranchesChangedTextId
        };
    }

    protected override string ResolveIssueSource() => string.Join("\n", _branchNames);

    private void SetParameter()
    {
        Parameters.Clear();
        Parameters.Add(StatusTextId(_branchNames.Count == 1));
        Parameters.AddRange(Truncate(_branchNames));
    }

    private static IEnumerable<string> Truncate(ICollection<string> branchNames)
    {
        if (branchNames.Count <= 1)
            return branchNames;

        var truncationLength = 0;
        var maxLength = branchNames.Max(x => x.Length);
        for (var i = 0; i < maxLength; i++)
        {
            char? currentChar = null;
            var allTheSame = true;
            foreach (var branchName in branchNames)
            {
                // this will happen, when a name is a substring of another name. In this case we cannot detect what to truncate
                if (i >= branchName.Length)
                    return branchNames;

                if (currentChar == null)
                {
                    currentChar = branchName[i];
                    continue;
                }

                if (branchName[i] != currentChar)
                {
                    allTheSame = false;
                    break;
                }
            }

            if (allTheSame && currentChar != null)
                truncationLength++;
            else
                break;
        }

        return branchNames.Select(s => s.Remove(0, truncationLength));
    }

    private readonly IList<string> _branchNames;

    // Branch {1} {0}. E.g. Branch stage failed.
    public const string BranchChangedTextId = nameof(BranchChangedTextId);

    // Branches {1}, {2} {3} {0}. E.g. Branch stage, master and featureA failed.
    public const string ThreeBranchesChangedTextId = nameof(ThreeBranchesChangedTextId);

    // Branches {1} and {2} {0}. E.g. Branch stage and master failed.
    public const string TwoBranchesChangedTextId = nameof(TwoBranchesChangedTextId);
}