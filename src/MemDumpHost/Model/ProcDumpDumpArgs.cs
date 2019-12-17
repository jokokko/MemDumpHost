namespace MemDumpHost.Model
{
    public sealed class ProcDumpDumpArgs
    {
        public string Args { get; private set; }

        public ProcDumpDumpArgs FullDump()
        {
            Args = string.Join(" ", Args, "-ma");
            return this;
        }
        public ProcDumpDumpArgs DumpUsingClone()
        {
            Args = string.Join(" ", Args, "-r");
            return this;
        }
        public ProcDumpDumpArgs OtherArguments(string args)
        {
            Args = string.Join(" ", Args, args);
            return this;
        }

        public ProcDumpDumpArgs AcceptEula()
        {
            Args = string.Join(" ", Args, "-accepteula");
            return this;
        }
    }
}