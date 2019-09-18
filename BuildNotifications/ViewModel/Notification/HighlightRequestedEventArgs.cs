using System;
using System.Collections.Generic;
using BuildNotifications.Core.Pipeline.Tree;

namespace BuildNotifications.ViewModel.Notification
{
    public class HighlightRequestedEventArgs : EventArgs
    {
        public HighlightRequestedEventArgs(IList<IBuildNode> buildNodes)
        {
            BuildNodes = buildNodes;
        }

        public IList<IBuildNode> BuildNodes { get; set; }
    }
}