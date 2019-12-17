using System;
using System.Diagnostics;
using System.IO;
using Process = System.Diagnostics.Process;

namespace MemDumpHost.Services
{
    public sealed class ProcessDumper
    {
        private const string ProcDump = "procdump.exe";
        private readonly string procDump;
        public ProcessDumper()
        {
            if (!NativeMethods.FindInPath(ProcDump, out procDump))
            {
                throw new InvalidOperationException($"No {ProcDump} in path");
            }
        }

        public ProcessDumper(string procDumpPath)
        {
            procDump = procDumpPath;
        }

        public string DumpSelf(string dumpFlags = null, string pathToDump = null)
        {
            pathToDump = pathToDump ?? Path.GetTempPath();
            var pid = Process.GetCurrentProcess().Id;
            var dumpArgs = string.Join(" ", dumpFlags, pid.ToString(), pathToDump);
            using (var dumpProcess = new Process
            {
                StartInfo = new ProcessStartInfo(procDump)
                {
                    Arguments = dumpArgs,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            })
            {
                dumpProcess.Start();
                dumpProcess.WaitForExit();
                
                var output = dumpProcess.StandardOutput.ReadToEnd();

                // To not resolve the various exit codes of ProcDump
                // ReSharper disable once InvertIf
                if (string.IsNullOrEmpty(output))
                {
                    var error = dumpProcess.StandardError.ReadToEnd();
                    throw new InvalidOperationException($"Exit code {dumpProcess.ExitCode}: {error}");
                }

                return output;
            }
        }
    }
}
