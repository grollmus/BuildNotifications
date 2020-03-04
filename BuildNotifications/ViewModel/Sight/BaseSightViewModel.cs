using System.Collections.Generic;
using BuildNotifications.Core.Text;
using BuildNotifications.PluginInterfaces.Builds.Sight;
using BuildNotifications.Resources.Icons;
using BuildNotifications.ViewModel.Tree;

namespace BuildNotifications.ViewModel.Sight
{
    public abstract class BaseSightViewModel : BaseViewModel
    {
        public ISight Sight { get; }
        private bool _isEnabled;
        private readonly string _toolTipTextId;

        public IconType Icon { get; }

        public string ToolTip => StringLocalizer.Instance.GetText(_toolTipTextId);

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                Sight.IsEnabled = value;
                OnPropertyChanged();
            }
        }

        protected BaseSightViewModel(ISight sight, IconType icon, string toolTipTextId)
        {
            Icon = icon;
            _toolTipTextId = toolTipTextId;
            Sight = sight;
        }

        public IEnumerable<BuildNodeViewModel> Apply(IEnumerable<BuildNodeViewModel> forBuilds)
        {
            foreach (var buildViewModel in forBuilds)
            {
                var isShown = Sight.IsBuildShown(buildViewModel.Node.Build);
                if (!isShown)
                    continue;

                var highlighted = Sight.IsHighlighted(buildViewModel.Node.Build);
                if (highlighted)
                    buildViewModel.IsHighlightedBySight = true;

                yield return buildViewModel;
            }
        }
    }
}