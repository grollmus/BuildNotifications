using DummyBuildServer.Models;
using DummyBuildServer.ViewModels;

namespace DummyBuildServer
{
    internal static class Kernel
    {
        internal static MainViewModel MainViewModel()
        {
            var serializer = new DataSerializer();

            return new MainViewModel(serializer);
        }
    }
}