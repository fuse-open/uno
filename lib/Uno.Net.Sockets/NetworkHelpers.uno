using Uno.Compiler.ExportTargetInterop;

namespace Uno.Net
{
    [extern(MSVC) Require("Source.Include", "winsock2.h")]
    [extern(MSVC) Require("LinkLibrary", "ws2_32")]
    [extern(UNIX) Require("Source.Include", "errno.h")]
    extern(CPLUSPLUS) internal class NetworkHelpers
    {
        extern(MSVC) public static string GetError()
        @{
            int err = WSAGetLastError();
            if (err == 0)
                return uString::Utf8("Unknown error");

            LPSTR buf = nullptr;
            DWORD size = FormatMessageA(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
                                        nullptr, err, MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), (LPSTR)&buf, 0, nullptr);
            if (size == 0)
                return uString::Utf8("Unknown error (FormatMessage failed)");

            uString *ret = uString::Utf8(buf, size);
            LocalFree(buf);
            return ret;
        @}

        extern(UNIX) public static string GetError()
        @{
            return uString::Utf8(strerror(errno));
        @}

        extern(MSVC) public static void EnsureWinsockInitialized()
        @{
            static bool winsockInitialized;
            if (winsockInitialized)
                return;

            WSADATA wsaData;
            int result = WSAStartup(MAKEWORD(2, 2), &wsaData);
            if (result != 0)
                U_THROW(@{Uno.Exception(string):New(uString::Utf8("WSAStartup failed"))});

            winsockInitialized = true;
        @}
    }
}
