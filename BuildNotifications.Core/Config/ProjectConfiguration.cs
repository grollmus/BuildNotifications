﻿using System;
using System.Collections.Generic;
using BuildNotifications.Core.Text;
using ReflectSettings.Attributes;

namespace BuildNotifications.Core.Config
{
    internal class ProjectConfiguration : IProjectConfiguration
    {
        public ProjectConfiguration()
        {
            BranchBlacklist = new List<string>();
            BranchWhitelist = new List<string>();
            BuildDefinitionBlacklist = new List<string>();
            BuildDefinitionWhitelist = new List<string>();
            BuildConnectionNames = new List<string>();
            SourceControlConnectionNames = new List<string>();

            ProjectName = StringLocalizer.NewProject;
            DefaultCompareBranch = string.Empty;

            PullRequestDisplay = PullRequestDisplayMode.Number;
            HideCompletedPullRequests = true;
            IsEnabled = true;
        }

        public bool IsEnabled { get; set; }

        [IsDisplayName]
        public string ProjectName { get; set; }

        [CalculatedValues(nameof(Configuration.ConnectionNames), true)]
        public IList<string> BuildConnectionNames { get; set; }

        [CalculatedValues(nameof(Configuration.ConnectionNames), true)]
        public IList<string> SourceControlConnectionNames { get; set; }

        [TypesForInstantiation(typeof(List<string>))]
        public IList<string> BranchBlacklist { get; set; }

        [TypesForInstantiation(typeof(List<string>))]
        public IList<string> BranchWhitelist { get; set; }

        public IList<string> BuildDefinitionBlacklist { get; set; }

        [TypesForInstantiation(typeof(List<string>))]
        public IList<string> BuildDefinitionWhitelist { get; set; }

        public string DefaultCompareBranch { get; set; }

        public bool HideCompletedPullRequests { get; set; }

        [IsHidden]
        [Obsolete]
        public bool ShowPullRequests { get; set; }

        public PullRequestDisplayMode PullRequestDisplay { get; set; }
    }
}