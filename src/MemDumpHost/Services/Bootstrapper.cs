using System.Threading.Tasks;
using MemDumpHost.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace MemDumpHost.Services
{
    public sealed class Bootstrapper
    {
        private IWebHost host;

        public Task Start(DumpEndpointSettings settings = null)
        {
            settings = settings ?? new DumpEndpointSettings();
            host = new WebHostBuilder()
                .UseUrls(settings.Url ?? "http://*:8080")
                .ConfigureServices(c => c.AddSingleton(settings))
                .UseKestrel()
                .UseStartup<Startup>()
                .Build();

            return host.RunAsync();
        }

        public Task Stop()
        {
            return host.StopAsync();
        }
    }
}