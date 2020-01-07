using System.Collections.Generic;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Core.Pipeline.Notification
{
    internal class DefinitionAndBranchNotification : BaseBuildNotification
    {
        public DefinitionAndBranchNotification(IList<IBuildNode> buildNodes, BuildStatus status, string definitionName, string branchName)
            : base(NotificationType.DefinitionAndBranch, buildNodes, status)
        {
            _definitionName = definitionName;
            _branchName = branchName;
            SetParameter();
        }

        protected override string GetMessageTextId()
        {
            return BranchAndDefinitionFailedTextId;
        }

        protected override string ResolveIssueSource()
        {
            return $"{_definitionName}\n{_branchName}";
        }

        private void SetParameter()
        {
            Parameters.Clear();
            Parameters.Add(StatusTextId(true));
            Parameters.Add(_definitionName);
            Parameters.Add(_branchName);
        }

        private readonly string _definitionName;
        private readonly string _branchName;

        // {2} Builds on {2} {0}. E.g. 12 Ci Builds on stage failed.
        public const string BranchAndDefinitionFailedTextId = nameof(BranchAndDefinitionFailedTextId);
    }
}