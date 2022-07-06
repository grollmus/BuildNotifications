using System.Collections.Generic;
using BuildNotifications.PluginInterfaces;

namespace BuildNotifications.Core.Pipeline;

internal class UserIdentityList : IUserIdentityList
{
    public UserIdentityList()
    {
        IdentitiesOfCurrentUser = new List<IUser>();
    }

    public ICollection<IUser> IdentitiesOfCurrentUser { get; }
}