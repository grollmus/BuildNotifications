using BuildNotifications.Core.Pipeline.Tree;

namespace BuildNotifications.ViewModel.Tree;

public class DefinitionGroupNodeViewModel : BuildTreeNodeViewModel
{
    public DefinitionGroupNodeViewModel(IDefinitionGroupNode node)
        : base(node)
    {
        DefinitionName = node.Definition.Name;
    }

    public string DefinitionName { get; set; }

    protected override string CalculateDisplayName() => DefinitionName;
}