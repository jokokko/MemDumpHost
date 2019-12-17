using System.IO;
using System.Net.Http;
using MemDumpHost.Services;
using Xunit;

namespace MemDumpHost.Tests.Services
{
    public sealed class ProcessDumperTests
    {
        private readonly string procDumpBinaryPath;

        public ProcessDumperTests()
        {
            using (var client = new HttpClient())
            {
                var procDumpBytes = client.GetByteArrayAsync("https://live.sysinternals.com/procdump.exe").Result;
                procDumpBinaryPath = Path.Combine(Path.GetTempPath(), "procdump.exe");
                File.WriteAllBytes(procDumpBinaryPath, procDumpBytes);
            }
        }

        [Fact]
        public void CanDumpSelf()
        {
            var sut = new ProcessDumper(procDumpBinaryPath);
            var result = sut.DumpSelf();
            Assert.Matches(@"(?m)Dump (\d+) initiated\:\s(?<filename>.+)(?=[\r\n]|\z)", result);
        }
    }
}