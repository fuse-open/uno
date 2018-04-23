using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    [extern(DOTNET) DotNetType("System.Environment")]
    [extern(CPLUSPLUS) Require("Source.Include", "cstdlib")]
    [extern(CPLUSPLUS) Require("Source.Declaration", "extern uSStrong<uArray*> _CommandLineArgs;")]
    public static class Environment
    {
        public static string NewLine
        {
            get { return defined(WIN32) ? "\r\n" : "\n"; }
        }

        extern(!mobile) public static void Exit(int exitCode)
        {
            if defined(CPLUSPLUS)
                extern "exit($0)";
            else
                throw new NotSupportedException();
        }

        extern(!mobile) public static string[] GetCommandLineArgs()
        {
            if defined(CPLUSPLUS)
                return extern<string[]> "_CommandLineArgs" ?? new string[0];
            else
                throw new NotSupportedException();
        }

        extern(!mobile) public static string GetEnvironmentVariable(string variable)
        {
            if defined(CPLUSPLUS)
            @{
                uCString cstr($0);
                return uString::Utf8(getenv(cstr.Ptr));
            @}
            else
                throw new NotSupportedException();
        }
    }
}
