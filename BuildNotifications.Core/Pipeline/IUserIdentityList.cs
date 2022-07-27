using System.Collections.Generic;
using BuildNotifications.PluginInterfaces;

namespace BuildNotifications.Core.Pipeline;

public interface IUserIdentityList
{
    void Add(IUser user);
    
    IReadOnlyCollection<IUser> IdentitiesOfCurrentUser { get; }
    void Clear();
}