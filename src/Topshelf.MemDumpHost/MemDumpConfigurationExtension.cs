using System;
using MemDumpHost.Model;
using MemDumpHost.Services;
using Topshelf.ServiceConfigurators;

namespace Topshelf.MemDumpHost
{
    public static class MemDumpConfigurationExtension
    {
        public static Bootstrapper EnableMemDumpEndpoint(this ServiceConfigurator configurator, Action<DumpEndpointSettings> configure = null)
        {
            var settings = new DumpEndpointSettings();
            configure?.Invoke(settings);
            var bootstrapper = new Bootstrapper();
            
            configurator.AfterStartingService(async () =>
            {
                try
                {
                    await bootstrapper.Start(settings);
                }
                catch (Exception e) when (settings.IgnoreStartupFailure != null)
                {
                    settings.IgnoreStartupFailure(e);
                }
            });
            
            // ReSharper disable once ImplicitlyCapturedClosure
            configurator.AfterStoppingService(async () => await bootstrapper.Stop());

            return bootstrapper;
        }
    }
}