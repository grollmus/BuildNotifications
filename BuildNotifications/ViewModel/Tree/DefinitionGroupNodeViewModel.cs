using System;
using System.Threading.Tasks;
using System.Windows;
using BuildNotifications.Core.Pipeline.Tree;

namespace BuildNotifications.ViewModel.Tree
{
    public class DefinitionGroupNodeViewModel : BuildTreeNodeViewModel
    {
        private static Random rnd = new Random();

        public DefinitionGroupNodeViewModel(IDefinitionGroupNode node) : base(node)
        {
            DefinitionName = node?.Definition?.Name;
            AddAtRandom();
        }

        private async void AddAtRandom()
        {
            while (true)
            {
                await Task.Delay(10000);
                if (rnd.NextDouble() < 0.25)
                    Application.Current.Dispatcher.Invoke(() => { AddAndRemoveCommand.Execute(null); });
            }

            throw new System.NotImplementedException();
        }

        public string DefinitionName { get; set; }

        protected override string CalculateDisplayName() => DefinitionName;
    }
}