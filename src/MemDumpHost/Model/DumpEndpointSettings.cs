using System;
using System.Text.RegularExpressions;

namespace MemDumpHost.Model
{
    public sealed class DumpEndpointSettings
    {
        public string ProcDumpPath { get; private set; }
        public string DumpDirectory { get; private set; }
        public string Url { get; private set; }
        public Guid? PathGuid { get; private set; }
        public string DumpArgs { get; private set; }
        public Regex DumpDownloadRegex { get; private set; }
        public string ProcDumpDownloadUri { get; private set; }
        public DumpEndpointSettings UseUrl(string url)
        {
            Url = url;
            return this;
        }

        public DumpEndpointSettings WithProcDumpPath(string path)
        {
            ProcDumpPath = path;
            return this;
        }

        public DumpEndpointSettings DumpToDirectory(string directory)
        {
            DumpDirectory = directory;
            return this;
        }
        public DumpEndpointSettings RandomGuidInEndpointPath()
        {
            PathGuid = Guid.NewGuid();
            return this;
        }
        public DumpEndpointSettings RandomGuidInEndpointPath(Guid pathGuid)
        {
            PathGuid = pathGuid;
            return this;
        }
        public DumpEndpointSettings DumpUsingArguments(string args)
        {
            DumpArgs = args;
            return this;
        }
        public DumpEndpointSettings DumpUsingArguments(Action<ProcDumpDumpArgs> args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            var argsToConfigure = new ProcDumpDumpArgs();
            args(argsToConfigure);
            DumpArgs = argsToConfigure.Args;
            return this;
        }
        public DumpEndpointSettings EnableDumpDownload(string regexToMatchFiles = @"(?m)Dump (\d+) initiated\:\s(?<filename>.+)(?=[\r\n]|\z)")
        {
            if (regexToMatchFiles == null)
            {
                throw new ArgumentNullException(nameof(regexToMatchFiles));
            }

            DumpDownloadRegex = new Regex(regexToMatchFiles);
            return this;
        }
        public DumpEndpointSettings EnableDumpDownload(Regex regex)
        {
            DumpDownloadRegex = regex ?? throw new ArgumentNullException(nameof(regex));
            return this;
        }

        public DumpEndpointSettings DownloadProcDump(string downloadUri = "https://live.sysinternals.com/procdump.exe")
        {
            ProcDumpDownloadUri = downloadUri;
            return this;
        }
    }
}