using Uno;
using Uno.Collections;
using Uno.Net.Sockets;
using Uno.Compiler.ExportTargetInterop;

namespace Uno.Net
{
    [DotNetType("System.Net.EndPoint")]
    public abstract class EndPoint
    {
        public AddressFamily AddressFamily
        {
            get;
            private set;
        }

        internal EndPoint(AddressFamily addressFamily)
        {
            AddressFamily = addressFamily;
        }
    }

    [DotNetType("System.Net.IPEndPoint")]
    public class IPEndPoint : EndPoint
    {
        public int Port { get; private set; }

        public IPAddress Address { get; private set; }

        public IPEndPoint(IPAddress address, int port)
            : base(address.AddressFamily)
        {
            Address = address;
            Port = port;
        }

        public override string ToString()
        {
            return Address.ToString() + ":" + Port;
        }

    }

    [extern(UNIX) Require("Source.Include", "arpa/inet.h")]
    [extern(MSVC) Require("Source.Include", "ws2tcpip.h")]
    [extern(MSVC) Require("LinkLibrary", "ws2_32")]
    [DotNetType("System.Net.IPAddress")]
    public class IPAddress
    {

        public static readonly IPAddress Any =
            new IPAddress(new byte[] { 0x00, 0x00, 0x00, 0x00 });

        public static readonly IPAddress Broadcast =
            new IPAddress(new byte[] { 0xff, 0xff, 0xff, 0xff });

        public static readonly IPAddress Loopback =
            new IPAddress(new byte[] { 0x7f, 0x00, 0x00, 0x01 });

        public static readonly IPAddress IPv6Any =
            new IPAddress(new byte[]{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },0);

        public static readonly IPAddress IPv6Loopback =
            new IPAddress(new byte[]{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },0);

        public static readonly IPAddress IPv6None =
            new IPAddress(new byte[]{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },0);

        public long Address
        {
            get
            {
                if (AddressFamily != AddressFamily.InterNetwork)
                    throw new SocketException("Can only take Address of IPv4 addresses");

                return _address[0] |
                      (_address[1] << 8) |
                      (_address[2] << 16) |
                      (_address[3] << 24);
            }
        }

        readonly byte[] _address;
        readonly uint _scopeId;
        readonly AddressFamily _addressFamily;

        public long ScopeId { get { return _scopeId; } }
        public AddressFamily AddressFamily { get { return _addressFamily; } }

        public IPAddress(long address)
        {
            if (uint.MinValue >= address || address >= uint.MaxValue)
                throw new ArgumentOutOfRangeException("address");

            _addressFamily = AddressFamily.InterNetwork;

            _address = new byte[4];
            _address[0] = (byte)address;
            _address[1] = (byte)(address >> 8);
            _address[2] = (byte)(address >> 16);
            _address[3] = (byte)(address >> 24);
        }

        public IPAddress(byte[] octets)
        {
            if (octets.Length == 4)
                _addressFamily = AddressFamily.InterNetwork;
            else if (octets.Length == 16)
                _addressFamily = AddressFamily.InterNetworkV6;
            else
                throw new Exception("Invalid ip address-length: " + octets.Length);

            _address = new byte[octets.Length];
            for (int i = 0; i < octets.Length; ++i)
                _address[i] = octets[i];
        }

        public IPAddress(byte[] address, long scopeId)
        {
            if (address == null)
                throw new ArgumentNullException("address");
            if (address.Length != 16)
                throw new Exception("Invalid ipv6 address");

            if (int.MinValue > scopeId || scopeId > int.MaxValue)
                throw new ArgumentOutOfRangeException("scopeId");

            _addressFamily = AddressFamily.InterNetworkV6;

            _address = new byte[address.Length];
            for (int i = 0; i < address.Length; ++i)
                _address[i] = address[i];

            _scopeId = (uint)scopeId;
        }

        extern(CPLUSPLUS) private static byte[] ParseIPv6Address(string address)
        @{
            char *tmp = uAllocCStr($0);
            unsigned char buf[sizeof(struct in6_addr)];
            int err = inet_pton(AF_INET6, tmp, buf);
            free(tmp);

            if (err != 1)
                return nullptr;

            return uArray::New(@{byte[]:TypeOf}, int(sizeof(struct in6_addr)), buf);
        @}

        public static IPAddress Parse(string address)
        {
            if (address.IndexOf(':') != -1)
            {
                // IPv6 address!

                extern long scopeid = 0;
                int percent = address.IndexOf('%');
                if (percent != -1)
                {
                    string scopeString = address.Substring(percent + 1, address.Length - percent - 1);
                    if (scopeString.Length > 10)
                        throw new FormatException("An invalid IP address was specified");

                    foreach (char ch in scopeString)
                        if (!char.IsDigit(ch))
                            throw new FormatException("An invalid IP address was specified");

                    scopeid = long.Parse(scopeString);
                    address = address.Substring(0, percent);
                }

                if defined(CPLUSPLUS)
                {
                    var data = ParseIPv6Address(address);
                    if (data == null)
                        throw new SocketException("");

                    return new IPAddress(data, scopeid);
                } else
                    build_error;
            }

            var parts = address.Split(new [] { '.' });
            if (parts.Length != 4)
                throw new FormatException("Invalid IPv4 address");

            var octets = new byte[4];
            for (int i = 0; i < 4; ++i)
            {
                var tmp = Int.Parse(parts[i]);
                if (byte.MinValue > tmp || tmp > byte.MaxValue)
                    throw new FormatException("Invalid IPv4 address");

                octets[i] = (byte)tmp;
            }

            return new IPAddress(octets);
        }

        public byte[] GetAddressBytes()
        {
            var address = new byte[_address.Length];
            for (int i = 0; i < _address.Length; i++)
                address[i] = _address[i];
            return address;
        }

        extern(CPLUSPLUS) private static string IPv6AddressToString(byte[] bytes)
        @{
            in6_addr addr;
            memcpy(addr.s6_addr, bytes->Ptr(), bytes->Length());

            char buf[INET6_ADDRSTRLEN];
            const char *ret = inet_ntop(AF_INET6, &addr, buf, INET6_ADDRSTRLEN);
            if (ret == nullptr)
                return nullptr;

            return uString::Utf8(ret);
        @}

        public override string ToString()
        {
            switch (AddressFamily)
            {
                case AddressFamily.InterNetwork:
                    var o1 = _address[0];
                    var o2 = _address[1];
                    var o3 = _address[2];
                    var o4 = _address[3];

                    return o1 + "." + o2 + "." + o3 + "." + o4;

                case AddressFamily.InterNetworkV6:
                    string ret;
                    if defined(CPLUSPLUS)
                        ret = IPv6AddressToString(_address);
                    else
                        build_error;
                    if (_scopeId != 0)
                        ret = string.Format("{0}%{1}", ret, _scopeId);
                    return ret;

                default:
                    throw new Exception("invalid AddressFamily");
            }
        }

        public override bool Equals(object obj)
        {
            var ip = obj as IPAddress;
            if (ip == null)
                return false;

            if (_addressFamily != ip._addressFamily)
                return false;

            if (_address.Length != ip._address.Length)
                return false;

            for (int i = 0; i < _address.Length; i++)
                if (_address[i] != ip._address[i])
                    return false;

            if (_scopeId != ip._scopeId)
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            int hash = 17 * 37 + _addressFamily.GetHashCode();

            hash = (hash * 37) + _address.Length;
            for (int i = 0; i < _address.Length; i++)
                hash = (hash * 37) + _address[i];

            hash = (hash * 37) + (int)_scopeId;
            return hash;
        }

    }
}
