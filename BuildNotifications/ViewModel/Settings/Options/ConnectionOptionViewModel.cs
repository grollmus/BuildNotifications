using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Core.Config;

namespace BuildNotifications.ViewModel.Settings.Options
{
    public class ConnectionOptionViewModel : ListOptionBaseViewModel<ConnectionData>
    {
        public ConnectionOptionViewModel(string displayName, IEnumerable<ConnectionData> connections, ConnectionData value)
            : base(displayName, value)
        {
            ModelValues = connections.ToList();
        }

        protected override IEnumerable<ConnectionData> ModelValues { get; }

        protected override string DisplayNameFor(ConnectionData item) => item.Name;
    }
}