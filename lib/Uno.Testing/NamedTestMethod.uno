namespace Uno.Testing
{
    class NamedTestMethod
    {
        public readonly Action Method;
        public readonly string Name;
        public readonly bool Ignore;
        public readonly string IgnoreReason;
        public bool Finished;
        public Exception Exception;

        public NamedTestMethod(Action method, string name, bool ignore, string ignoreReason)
        {
            Method = method;
            Name = name;
            Ignore = ignore;
            IgnoreReason = ignoreReason;
        }
    }
}
