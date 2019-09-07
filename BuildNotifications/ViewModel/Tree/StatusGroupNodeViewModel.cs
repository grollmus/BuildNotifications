using System;
using BuildNotifications.Core.Pipeline.Tree;

namespace BuildNotifications.ViewModel.Tree
{
    public class StatusGroupNodeViewModel : BuildTreeNodeViewModel
    {
        public StatusGroupNodeViewModel(IBuildTreeNode node) : base(node)
        {
        }

        protected override string CalculateDisplayName() => throw new NotImplementedException();
    }
}