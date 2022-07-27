using System.Collections.Generic;
using System.Linq;
using BuildNotifications.PluginInterfaces;

namespace BuildNotifications.Core.Pipeline;

internal class UserIdentityList : IUserIdentityList
{
    public void Clear() => _userList.Clear();

    public void Add(IUser user)
    {
        if (_userList.All(u => u.Id != user.Id))
            _userList.Add(user);
    }

    public IReadOnlyCollection<IUser> IdentitiesOfCurrentUser => _userList;

    private readonly List<IUser> _userList = new();
}