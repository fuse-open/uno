namespace Uno.Compiler.Core
{
    public struct CompilerOptions
    {
        public bool Debug;
        public bool CanCacheIL;
        public bool CodeCompletionMode;
        public bool Parallel;
        public bool Strip;
        public string OutputDirectory;
        public string MainClass;
        public string BuildTarget;
        public string Configuration;
        public TestOptions? TestOptions;
    }
}
