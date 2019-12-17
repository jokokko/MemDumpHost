using System;
using MemDumpHost.Model;
using MemDumpHost.Services;
using Topshelf.ServiceConfigurators;

namespace Topshelf.MemDumpHost
{
    public static class MemDumpConfigurationExtension
    {
        public static void EnableMemDumpEndpoint(this ServiceConfigurator configurator, Action<DumpEndpointSettings> configure = null)
        {
            var settings = new DumpEndpointSettings();
            configure?.Invoke(settings);
            var bootstrapper = new Bootstrapper();
            
            configurator.AfterStartingService(async () => await bootstrapper.Start(settings));
            configurator.AfterStoppingService(async () => await bootstrapper.Stop());
        }
    }
}