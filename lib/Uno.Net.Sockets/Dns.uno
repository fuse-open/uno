using Uno;
using Uno.Collections;
using Uno.Compiler.ExportTargetInterop;
using Uno.Net.Sockets;

namespace Uno.Net
{
    [DotNetType("System.Net.Dns")]
    [extern(APPLE || LINUX) Require("Source.Include", "ifaddrs.h")]
    [extern(UNIX) Require("Source.Include", "sys/socket.h")]
    [extern(UNIX) Require("Source.Include", "netdb.h")]
    [extern(UNIX) Require("Source.Include", "netinet/in.h")]
    [extern(MSVC) Require("Source.Include", "ws2tcpip.h")]
    [Require("Source.Include", "vector")]
    [ForeignInclude(Language.Java, "java.util.*", "java.net.*")]
    public class Dns
    {
        extern(APPLE || LINUX) static IPAddress[] GetLocalAddresses()
        @{
            ifaddrs *addr, *curr;
            if (getifaddrs(&addr) != 0)
                return nullptr;

            std::vector<@{IPAddress}> addresses;
            for (curr = addr; curr; curr = curr->ifa_next)
            {
                sockaddr *sa = curr->ifa_addr;
                if (!sa)
                    continue;

                if (sa->sa_family == AF_INET)
                {
                    sockaddr_in *sa4 = (sockaddr_in *)sa;
                    @{byte[]} tmp = uArray::New(@{byte[]:TypeOf}, 4, &sa4->sin_addr);
                    addresses.push_back(@{IPAddress(byte[]):New(tmp)});
                }
                else if (sa->sa_family == AF_INET6)
                {
                    sockaddr_in6 *sa6 = (sockaddr_in6 *)sa;
                    @{byte[]} tmp = uArray::New(@{byte[]:TypeOf}, 16, &sa6->sin6_addr);
                    addresses.push_back(@{IPAddress(byte[]):New(tmp)});
                }
            }
            freeifaddrs(addr);

            return uArray::New(@{IPAddress[]:TypeOf}, addresses.size(), &addresses[0]);
        @}

        [Foreign(Language.Java)]
        extern(ANDROID) static bool JavaGetLocalAddresses(List<string> addresses)
        @{
            try
            {
                Enumeration<NetworkInterface> networkInterfaces = NetworkInterface.getNetworkInterfaces();
                while (networkInterfaces.hasMoreElements())
                {
                    NetworkInterface networkInterface = networkInterfaces.nextElement();
                    Enumeration<InetAddress> inetAddresses = networkInterface.getInetAddresses();
                    while (inetAddresses.hasMoreElements())
                    {
                        InetAddress inetAddress = inetAddresses.nextElement();
                        String hostAddress = inetAddress.getHostAddress();
                        if (inetAddress instanceof Inet6Address)
                        {
                            Inet6Address inet6Address = (Inet6Address)inetAddress;
                            int delim = hostAddress.indexOf('%');
                            if (delim >= 0)
                            {
                                // we only want numerical scopeIDs
                                hostAddress = hostAddress.substring(0, delim + 1);
                                hostAddress += inet6Address.getScopeId();
                            }
                        }
                        @{List<string>:Of(addresses).Add(string):Call(hostAddress)};
                    }
                }
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        @}

        extern(ANDROID) static IPAddress[] GetLocalAddresses()
        {
            var result = new List<string>();
            if (!JavaGetLocalAddresses(result))
                return null;

            var ret = new IPAddress[result.Count];
            for (int i = 0 ; i < result.Count; ++i)
                ret[i] = IPAddress.Parse(result[i]);

            return ret;
        }

        static IPAddress[] GetHostAddressesImpl(string hostNameOrAddress)
        @{
            char *tmp = uAllocCStr(hostNameOrAddress);
            struct addrinfo *addr, *curr;
            int error = getaddrinfo(tmp, nullptr, nullptr, &addr);
            free(tmp);
            if (error != 0)
                return nullptr;

            std::vector<@{IPAddress}> addresses;
            for (curr = addr; curr; curr = curr->ai_next)
            {
                if (curr->ai_family == AF_INET)
                {
                    struct sockaddr_in *sa = (struct sockaddr_in *)curr->ai_addr;
                    @{byte[]} tmp = uArray::New(@{byte[]:TypeOf}, 4, &sa->sin_addr);
                    addresses.push_back(@{IPAddress(byte[]):New(tmp)});
                }
                else if (curr->ai_family == AF_INET6)
                {
                    struct sockaddr_in6 *sa = (struct sockaddr_in6 *)curr->ai_addr;
                    @{byte[]} tmp = uArray::New(@{byte[]:TypeOf}, 16, &sa->sin6_addr);
                    addresses.push_back(@{IPAddress(byte[]):New(tmp)});
                }
            }
            freeaddrinfo(addr);

            return uArray::New(@{IPAddress[]:TypeOf}, addresses.size(), &addresses[0]);
        @}

        public static IPAddress[] GetHostAddresses(string hostNameOrAddress)
        {
            if (hostNameOrAddress == null)
                throw new ArgumentNullException("hostNameOrAddress");

            if (hostNameOrAddress.Length > 255 ||
                (hostNameOrAddress.Length == 255 && hostNameOrAddress[254] != '.'))
                throw new ArgumentOutOfRangeException("hostNameOrAddress");

            if defined(MSVC)
                NetworkHelpers.EnsureWinsockInitialized();

            if defined(CPLUSPLUS)
            {
                if defined(UNIX)
                {
                    if (hostNameOrAddress.Length == 0)
                    {
                        var localAddresses = GetLocalAddresses();
                        if (localAddresses == null)
                            throw new SocketException(NetworkHelpers.GetError());
                        return localAddresses;
                    }
                }

                var ret = GetHostAddressesImpl(hostNameOrAddress);
                if (ret == null)
                    throw new SocketException(NetworkHelpers.GetError());
                return ret;
            }
            else
                build_error;
        }
    }
}
