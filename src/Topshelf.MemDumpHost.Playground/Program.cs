using System;
using Topshelf.Logging;

namespace Topshelf.MemDumpHost.Playground
{
    public sealed class Program
    {
        public static void Main(string[] args)
        {
            var rc = HostFactory.Run(x =>
            {
                x.Service<TownCrier>(s =>
                {
                    s.ConstructUsing(name => new TownCrier());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                    s.EnableMemDumpEndpoint(c =>
                    {
                        c
                            // Without download or explicit path options, ProcDump is resolved from path env variable.
                            .DownloadProcDump()
                            // Or .WithProcDumpPath(@"c:\temp\procdump.exe")
                            .EnableDumpDownload()
                            .DumpToDirectory(@"c:\temp")
                            .UseUrl("http://*:8080")
                            .RandomGuidInEndpointPath()
                            // Or .RandomGuidInEndpointPath(Guid.Parse("B80B3FFF-EF23-4D7A-A1DF-A93F7A979177"))
                            .DumpUsingArguments(a => a.FullDump().DumpUsingClone().AcceptEula());

                        HostLogger.Get<Program>().Info($"Dump uri guid: {c.PathGuid}");
                    });
                });
                x.RunAsLocalSystem();

                x.SetDescription("Sample Topshelf Host with MemDumpHost enabled");
                x.SetDisplayName("Stuff");
                x.SetServiceName("Stuff");
            });

            var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());
            Environment.ExitCode = exitCode;
        }
    }
}