using System.Collections.Generic;
using BuildNotifications.PluginInterfaces;

namespace BuildNotifications.Core.Pipeline
{
    public interface IUserIdentityList
    {
        ICollection<IUser> IdentitiesOfCurrentUser { get; }
    }
}