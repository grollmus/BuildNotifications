using System.Runtime.Serialization;
using BuildNotifications.Plugin.DummyServer;
using BuildNotifications.PluginInterfaces.Builds;

namespace DummyServer.Controllers;

[DataContract]
public class WebBuild
{
    public WebBuild(Build build)
    {
        BranchName = build.BranchName;
        DefinitionName = build.Definition.Name;
        UserName = build.RequestedBy.UniqueName;
        Id = build.Id;
        Status = build.Status;
        Reason = build.Reason;
    }

    public WebBuild()
    {
        // needed for serialization   
    }

    [DataMember]
    public string BranchName { get; set; }

    [DataMember]
    public string DefinitionName { get; set; }

    [DataMember]
    public string Id { get; set; }

    [DataMember]
    public BuildReason Reason { get; set; }

    [DataMember]
    public BuildStatus Status { get; set; }

    [DataMember]
    public string UserName { get; set; }
}