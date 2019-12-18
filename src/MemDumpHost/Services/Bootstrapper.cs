using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using MemDumpHost.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace MemDumpHost.Services
{
    public sealed class Bootstrapper
    {
        private IWebHost host;

        public ProcessDumper Dumper { get; private set; }

        public Task Start(DumpEndpointSettings settings = null)
        {
            settings = settings ?? new DumpEndpointSettings();

            var procDumpBinaryPath = settings.ProcDumpPath;

            if (!string.IsNullOrEmpty(settings.ProcDumpDownloadUri))
            {
                using (var client = new HttpClient())
                {
                    var procDumpBytes = client.GetByteArrayAsync(settings.ProcDumpDownloadUri).Result;
                    procDumpBinaryPath = Path.Combine(Path.GetTempPath(), "procdump.exe");
                    File.WriteAllBytes(procDumpBinaryPath, procDumpBytes);
                }
            }

            Dumper = !string.IsNullOrEmpty(procDumpBinaryPath) ? new ProcessDumper(procDumpBinaryPath) : new ProcessDumper();

            host = new WebHostBuilder()
                .UseUrls(settings.Url ?? "http://*:8080")
                .ConfigureServices(c => c.AddSingleton(settings).AddSingleton(Dumper))
                .UseKestrel()
                .UseStartup<Startup>()
                .Build();

            return host.RunAsync();
        }

        public Task Stop()
        {
            return host?.StopAsync();
        }
    }
}