using System;
using System.Timers;

namespace Topshelf.MemDumpHost.Playground
{
    public class TownCrier
    {
        private readonly Timer timer;
        public TownCrier()
        {
            timer = new Timer(1000) { AutoReset = true };
            timer.Elapsed += (sender, eventArgs) => Console.WriteLine("It is {0} and all is well", DateTime.Now);
        }
        public void Start() { timer.Start(); }
        public void Stop() { timer.Stop(); }
    }
}