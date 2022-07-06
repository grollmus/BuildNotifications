using System;
using System.Threading.Tasks;
using BuildNotifications.PluginInterfaces;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.Configuration;
using BuildNotifications.PluginInterfaces.Host;
using BuildNotifications.PluginInterfaces.SourceControl;
using JetBrains.Annotations;

namespace BuildNotifications.Plugin.GitHub;

[PublicAPI]
public class GitHubPlugin : ISourceControlPlugin, IBuildPlugin
{
    async Task<ConnectionTestResult> IBuildPlugin.TestConnection(string serialized)
    {
        await Task.Yield();
        throw new NotImplementedException();
    }

    IBuildProvider IBuildPlugin.ConstructProvider(string serialized) => throw new NotImplementedException();

    public IPluginConfiguration ConstructNewConfiguration() => throw new NotImplementedException();

    public string DisplayName => "GitHub";
    public string IconSvgPath => "M 16.288,0 C 7.2929999,0 0,7.293 0,16.29 c 0,7.197 4.667,13.302 11.14,15.457 0.815,0.149 1.112,-0.354 1.112,-0.786 0,-0.386 -0.014,-1.411 -0.022,-2.77 C 7.6989999,29.175 6.7429999,26.007 6.7429999,26.007 6.0019999,24.126 4.934,23.625 4.934,23.625 3.455,22.614 5.046,22.634 5.046,22.634 c 1.6349999,0.116 2.4949999,1.679 2.4949999,1.679 1.453,2.489 3.8130001,1.77 4.7410001,1.354 0.148,-1.053 0.568,-1.771 1.034,-2.178 -3.6170001,-0.411 -7.4200001,-1.809 -7.4200001,-8.051 0,-1.778 0.635,-3.232 1.677,-4.371 -0.168,-0.412 -0.727,-2.068 0.159,-4.311 0,0 1.368,-0.438 4.4800001,1.67 1.299,-0.361 2.693,-0.542 4.078,-0.548 1.383,0.006 2.777,0.187 4.078,0.548 3.11,-2.108 4.475,-1.67 4.475,-1.67 0.889,2.243 0.33,3.899 0.162,4.311 1.044,1.139 1.675,2.593 1.675,4.371 0,6.258 -3.809,7.635 -7.438,8.038 0.585,0.503 1.106,1.497 1.106,3.017 0,2.177 -0.02,3.934 -0.02,4.468 0,0.436 0.293,0.943 1.12,0.784 C 27.916,29.586 32.579,23.485 32.579,16.29 32.579,7.293 25.285,0 16.288,0";

    public void OnPluginLoaded(IPluginHost host)
    {
        // nothing to do
    }

    IBranchProvider ISourceControlPlugin.ConstructProvider(string serialized) => throw new NotImplementedException();

    async Task<ConnectionTestResult> ISourceControlPlugin.TestConnection(string serialized)
    {
        await Task.Yield();
        throw new NotImplementedException();
    }
}