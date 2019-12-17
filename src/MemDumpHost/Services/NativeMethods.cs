using System;
using System.Runtime.InteropServices;
using System.Text;

namespace MemDumpHost.Services
{
    internal static class NativeMethods
    {
        [DllImport("shlwapi.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern bool PathFindOnPath([MarshalAs(UnmanagedType.LPTStr)] StringBuilder pszFile, IntPtr unused);

        public static bool FindInPath(string pszFile, out string fullPath)
        {
            if (pszFile == null)
            {
                throw new ArgumentNullException(nameof(pszFile));
            }

            var sb = new StringBuilder(pszFile, 260);
            var found = PathFindOnPath(sb, IntPtr.Zero);
            fullPath = found ? sb.ToString() : null;
            return found;
        }
    }
}