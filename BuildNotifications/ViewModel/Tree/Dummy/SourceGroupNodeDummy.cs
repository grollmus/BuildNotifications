using BuildNotifications.Core.Pipeline.Tree;

namespace BuildNotifications.ViewModel.Tree.Dummy
{
    internal class SourceGroupNodeDummy : BuildTreeNodeDummy, ISourceGroupNode
    {
        public SourceGroupNodeDummy(string sourceName)
        {
            SourceName = sourceName;
        }

        public string SourceName { get; set; }
    }
}