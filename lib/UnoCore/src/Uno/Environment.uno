using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    [extern(DOTNET) DotNetType("System.Environment")]
    [extern(CPLUSPLUS) Require("source.include", "cstdlib")]
    [extern(CPLUSPLUS && !WIN32) Require("source.declaration", "extern int uArgc;")]
    [extern(CPLUSPLUS && !WIN32) Require("source.declaration", "extern char** uArgv;")]
    [extern(WIN32) Require("source.include", "uno/WinAPI.h")]
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
            if defined(WIN32)
            @{
                int numArgs;
                LPWSTR* cmdArgs = CommandLineToArgvW(GetCommandLineW(), &numArgs);

                if (numArgs < 2)
                    return uArray::New(@{string[]:typeof}, 0);

                @{string[]} args = uArray::New(@{string[]:typeof}, numArgs - 1);

                for (int i = 1; i < numArgs; i++)
                    args->Strong<uString*>(i - 1) = uString::Utf16((const char16_t*) cmdArgs[i]);

                return args;
            @}
            else if defined(CPLUSPLUS)
            @{
                if (uArgc < 2)
                    return uArray::New(@{string[]:typeof}, 0);

                @{string[]} args = uArray::New(@{string[]:typeof}, uArgc - 1);

                for (int i = 1; i < uArgc; i++)
                    args->Strong<uString*>(i - 1) = uString::Utf8(uArgv[i]);

                return args;
            @}
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
