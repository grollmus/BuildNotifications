using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.PluginInterfaces;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Core.Pipeline.Notification
{
    internal class NotificationFactory
    {
        /// <summary>
        /// Factory to create Notifications from a BuildTreeDelta.
        /// </summary>
        /// <param name="configuration">Configuration needed to filter which notifications should be created.</param>
        /// <param name="userIdentityList">List of identities for the current user.</param>
        public NotificationFactory(IConfiguration configuration, IUserIdentityList userIdentityList)
        {
            _configuration = configuration;
            _userIdentityList = userIdentityList;
        }

        public IEnumerable<INotification> ProduceNotifications(IBuildTreeBuildsDelta fromDelta)
        {
            if (NothingChanged(fromDelta))
                return Enumerable.Empty<INotification>();

            return CreateNotifications(fromDelta);
        }

        private INotification BranchNotification(List<IBuildNode> buildNodes, BuildStatus status, IEnumerable<string> branchNames)
        {
            return new BranchNotification(buildNodes, status, branchNames);
        }

        private INotification BuildsNotifications(IList<IBuildNode> buildNodes, BuildStatus status)
        {
            return new BuildNotification(buildNodes, status);
        }

        private IEnumerable<INotification> CreateNotifications(IBuildTreeBuildsDelta fromDelta)
        {
            var succeeded = FilterSucceeded(fromDelta).ToList();
            var failed = FilterFailed(fromDelta).ToList();
            var cancelled = FilterCancelled(fromDelta).ToList();

            var allGroups = new List<List<IBuildNode>> {succeeded, failed, cancelled};

            foreach (var buildNodes in allGroups.Where(x => x.Count > 0))
            {
                var status = buildNodes.Max(x => ParseStatus(x.Status));

                // try to group by definition/branch etc.
                var groupedByDefinitionAndBranch = GroupByDefinitionAndBranch(buildNodes).ToList();
                var groupedByDefinition = GroupByDefinition(buildNodes).ToList();
                var groupedByBranch = GroupByBranch(buildNodes).ToList();

                // only display groups with a size of up to 3 elements
                var allGroupings = new List<IEnumerable<object>> {groupedByDefinitionAndBranch, groupedByDefinition, groupedByBranch};
                var smallestCount = allGroupings.Min(x => x.Count());

                // for a single build, also display the build notification
                if (smallestCount > 3 || allGroups.SelectMany(x => x).Count() == 1)
                {
                    yield return BuildsNotifications(buildNodes, status);
                    continue;
                }

                // only give this message if it's exactly the same definition and branch for every build
                // otherwise there would be two many messages
                if (groupedByDefinitionAndBranch.Count == 1)
                {
                    var (definition, branch) = groupedByDefinitionAndBranch.First().Key;
                    yield return DefinitionAndBranchNotification(buildNodes, status, definition, branch);
                    continue;
                }

                if (groupedByDefinition.Count == smallestCount)
                {
                    yield return DefinitionNotification(buildNodes, status, groupedByDefinition.Select(x => x.Key));
                    continue;
                }

                yield return BranchNotification(buildNodes, status, groupedByBranch.Select(x => x.Key));
            }
        }

        private BuildStatus ParseStatus(BuildStatus buildStatus)
        {
            switch (buildStatus)
            {
                case BuildStatus.PartiallySucceeded:
                    return _configuration.PartialSucceededTreatmentMode switch
                    {
                        PartialSucceededTreatmentMode.TreatAsSucceeded => BuildStatus.Succeeded,
                        PartialSucceededTreatmentMode.TreatAsFailed => BuildStatus.Failed,
                        _ => buildStatus
                    };

                default:
                    return buildStatus;
            }
        }

        private INotification DefinitionAndBranchNotification(List<IBuildNode> buildNodes, BuildStatus status, string tupleDefinition, string tupleBranch)
        {
            return new DefinitionAndBranchNotification(buildNodes, status, tupleDefinition, tupleBranch);
        }

        private INotification DefinitionNotification(IList<IBuildNode> buildNodes, BuildStatus status, IEnumerable<string> definitionNames)
        {
            return new DefinitionNotification(buildNodes, status, definitionNames.ToList());
        }

        private IEnumerable<IBuildNode> FilterCancelled(IBuildTreeBuildsDelta fromDelta)
        {
            var failed = fromDelta.Failed.Where(ShouldNotifyAboutBuild).ToList();
            var succeeded = fromDelta.Succeeded.Where(ShouldNotifyAboutBuild);
            var cancelled = fromDelta.Cancelled.Where(ShouldNotifyAboutBuild);

            if (failed.Any() && !succeeded.Any())
                return Enumerable.Empty<IBuildNode>();

            return cancelled;
        }

        private IEnumerable<IBuildNode> FilterFailed(IBuildTreeBuildsDelta fromDelta)
        {
            var failed = fromDelta.Failed.Where(ShouldNotifyAboutBuild).ToList();
            var succeeded = fromDelta.Succeeded.Where(ShouldNotifyAboutBuild);
            var cancelled = fromDelta.Cancelled.Where(ShouldNotifyAboutBuild);

            if (failed.Any() && !succeeded.Any())
                return failed.Concat(cancelled);

            return failed;
        }

        private IEnumerable<IBuildNode> FilterSucceeded(IBuildTreeBuildsDelta fromDelta)
        {
            return fromDelta.Succeeded.Where(ShouldNotifyAboutBuild);
        }

        private IEnumerable<IGrouping<string, IBuildNode>> GroupByBranch(IEnumerable<IBuildNode> allBuilds)
        {
            return allBuilds.GroupBy(x => x.Build.BranchName);
        }

        private IEnumerable<IGrouping<string, IBuildNode>> GroupByDefinition(IEnumerable<IBuildNode> allBuilds)
        {
            return allBuilds.GroupBy(x => x.Build.Definition.Name);
        }

        private IEnumerable<IGrouping<(string definition, string branch), IBuildNode>> GroupByDefinitionAndBranch(IEnumerable<IBuildNode> allBuilds)
        {
            return allBuilds.GroupBy(x => (definition: x.Build.Definition.Name, branch: x.Build.BranchName));
        }

        private bool IsSameUser(IUser userA, IUser? userB)
        {
            return userA.Id == userB?.Id;
        }

        private bool NothingChanged(IBuildTreeBuildsDelta fromDelta)
        {
            return !fromDelta.Cancelled.Any()
                   && !fromDelta.Failed.Any()
                   && !fromDelta.Succeeded.Any();
        }

        private BuildNotificationModes NotifySetting(IBuildNode buildNode)
        {
            switch (buildNode.Status)
            {
                case BuildStatus.Failed:
                    return _configuration.FailedBuildNotifyConfig;
                case BuildStatus.Succeeded:
                    return _configuration.SucceededBuildNotifyConfig;
                default:
                    return _configuration.CanceledBuildNotifyConfig;
            }
        }

        private bool ShouldNotifyAboutBuild(IBuildNode buildNode)
        {
            if (buildNode.IsManualNotificationEnabled)
                return true;

            var notifySetting = NotifySetting(buildNode);
            var currentUserIdentities = _userIdentityList.IdentitiesOfCurrentUser;
            return notifySetting switch
            {
                BuildNotificationModes.None => false,
                BuildNotificationModes.RequestedByMe => currentUserIdentities.Any(u => IsSameUser(u, buildNode.Build.RequestedBy)),
                BuildNotificationModes.RequestedForMe => currentUserIdentities.Any(u => IsSameUser(u, buildNode.Build.RequestedFor)),
                BuildNotificationModes.RequestedByOrForMe => currentUserIdentities.Any(u => IsSameUser(u, buildNode.Build.RequestedFor) || IsSameUser(u, buildNode.Build.RequestedBy)),
                _ => true
            };
        }

        private readonly IConfiguration _configuration;
        private readonly IUserIdentityList _userIdentityList;
    }
}