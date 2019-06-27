using BuildNotifications.Core.Pipeline.Tree;

namespace BuildNotifications.ViewModel.Tree.Dummy
{
    internal class SourceGroupNodeDummy : BuildTreeNodeDummy, ISourceGroupNode
    {
        public string SourceName { get; set; }

        public SourceGroupNodeDummy(string sourceName)
        {
            SourceName = sourceName;
        }
    }
}