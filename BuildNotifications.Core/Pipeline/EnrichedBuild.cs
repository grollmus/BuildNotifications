﻿using System;
using BuildNotifications.PluginInterfaces;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Core.Pipeline
{
    internal class EnrichedBuild : IBuild
    {
        public EnrichedBuild(IBaseBuild build, string projectName, IBuildProvider provider)
        {
            OriginalBuild = build;
            ProjectName = projectName;
            Provider = provider;
            BranchName = OriginalBuild.BranchName;
        }

        internal IBranch? Branch { get; set; }

        public IBuildProvider Provider { get; }

        internal IBaseBuild OriginalBuild { get; }

        public bool Equals(IBaseBuild other)
        {
            return OriginalBuild.Equals(other);
        }

        public string ProjectName { get; }

        public string BranchName { get; set; }

        public IBuildDefinition Definition => OriginalBuild.Definition;

        public string Id => OriginalBuild.Id;

        public DateTime? LastChangedTime => OriginalBuild.LastChangedTime;

        public int Progress => OriginalBuild.Progress;

        public DateTime? QueueTime => OriginalBuild.QueueTime;

        public IUser RequestedBy => OriginalBuild.RequestedBy;

        public IUser? RequestedFor => OriginalBuild.RequestedFor;

        public BuildStatus Status => OriginalBuild.Status;

        public IBuildLinks Links => OriginalBuild.Links;
    }
}