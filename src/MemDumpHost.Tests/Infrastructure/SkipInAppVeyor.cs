using System;
using Xunit;

namespace MemDumpHost.Tests.Infrastructure
{
    // From: https://josephwoodward.co.uk/2019/01/skipping-xunit-tests-based-on-runtime-conditions
    public sealed class SkipInAppVeyor : FactAttribute
    {
        public SkipInAppVeyor(string msg)
        {
            if (IsAppVeyor())
            {
                Skip = msg;
            }
        }

        private static bool IsAppVeyor()
            => Environment.GetEnvironmentVariable("APPVEYOR") != null;
    }
}