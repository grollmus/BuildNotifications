using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Core.Config;

namespace BuildNotifications.ViewModel.Settings.Options;

public class ConnectionOptionViewModel : ListOptionBaseViewModel<ConnectionData>
{
    public ConnectionOptionViewModel(string displayName, IEnumerable<ConnectionData> connections, ConnectionData value)
        : base(displayName, value)
    {
        _connectionList = connections.ToList();
    }

    protected override IEnumerable<ConnectionData> ModelValues => _connectionList;

    public void SetAvailableConnections(IEnumerable<ConnectionData> connections)
    {
        _connectionList = connections.ToList();
        InvalidateAvailableValues();
    }

    protected override string DisplayNameFor(ConnectionData item) => item.Name;

    private List<ConnectionData> _connectionList;
}