using System;
using System.Threading.Tasks;
using BuildNotifications.PluginInterfaces;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;
using JetBrains.Annotations;

namespace BuildNotifications.Plugin.GitHub
{
    [PublicAPI]
    public class GitHubPlugin : ISourceControlPlugin, IBuildPlugin
    {
        async Task<ConnectionTestResult> IBuildPlugin.TestConnection(object data)
        {
            await Task.Yield();
            throw new NotImplementedException();
        }

        IBuildProvider? IBuildPlugin.ConstructProvider(object? data) => throw new NotImplementedException();

        public string DisplayName => "GitHub";
        public string IconSvgPath => "M 16.288,31.774867 C 7.2929999,31.774867 0,24.481867 0,15.484867 0,8.2878668 4.667,2.1828668 11.14,0.02786681 c 0.815,-0.149 1.112,0.354 1.112,0.786 0,0.38599999 -0.014,1.41099999 -0.022,2.76999999 -4.5310001,-0.984 -5.4870001,2.184 -5.4870001,2.184 -0.741,1.881 -1.8089999,2.382 -1.8089999,2.382 -1.479,1.011 0.112,0.991 0.112,0.991 1.6349999,-0.116 2.4949999,-1.679 2.4949999,-1.679 1.453,-2.489 3.8130001,-1.77 4.7410001,-1.354 0.148,1.053 0.568,1.771 1.034,2.178 -3.6170001,0.411 -7.4200001,1.8090002 -7.4200001,8.0510002 0,1.778 0.635,3.232 1.677,4.371 -0.168,0.412 -0.727,2.068 0.159,4.311 0,0 1.368,0.438 4.4800001,-1.67 1.299,0.361 2.693,0.542 4.078,0.548 1.383,-0.006 2.777,-0.187 4.078,-0.548 3.11,2.108 4.475,1.67 4.475,1.67 0.889,-2.243 0.33,-3.899 0.162,-4.311 1.044,-1.139 1.675,-2.593 1.675,-4.371 0,-6.258 -3.809,-7.6350002 -7.438,-8.0380002 0.585,-0.503 1.106,-1.497 1.106,-3.017 0,-2.177 -0.02,-3.934 -0.02,-4.46799999 0,-0.436 0.293,-0.943 1.12,-0.784 6.468,2.15899999 11.131,8.25999999 11.131,15.45500019 0,8.997 -7.294,16.29 -16.291,16.29";

        public void ConfigurationChanged()
        {
            throw new NotImplementedException();
        }

        public Type GetConfigurationType() => throw new NotImplementedException();

        public void SetCurrentConfiguration(object instance)
        {
            throw new NotImplementedException();
        }

        IBranchProvider? ISourceControlPlugin.ConstructProvider(object? data) => throw new NotImplementedException();

        async Task<ConnectionTestResult> ISourceControlPlugin.TestConnection(object data)
        {
            await Task.Yield();
            throw new NotImplementedException();
        }
    }
}