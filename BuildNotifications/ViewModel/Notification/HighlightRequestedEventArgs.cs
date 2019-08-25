using System;
using System.Collections.Generic;
using BuildNotifications.Core.Pipeline.Tree;

namespace BuildNotifications.ViewModel.Notification
{
    public class HighlightRequestedEventArgs : EventArgs
    {
        public IList<IBuildNode> BuildNodes { get; set; }

        public HighlightRequestedEventArgs(IList<IBuildNode> buildNodes)
        {
            BuildNodes = buildNodes;
        }
    }
}