using Uno;
using Uno.IO;
using Uno.Testing;
using Uno.Net.Sockets;

namespace Uno.Net.Sockets.Test
{
    public class IPAddressTest
    {
        void ConstructFromTooLarge()
        {
            Assert.AreNotEqual(0xFFFFFFFF, new IPAddress((long)uint.MaxValue + 1).Address);
        }

        void ConstructFromNegative()
        {
            Assert.AreNotEqual(0x80000000, new IPAddress(-1).Address);
        }

        [Test]
        [Ignore("https://github.com/fusetools/uno/issues/1509", "CIL && HOST_OSX")]
        public void FromLong()
        {
            Assert.Throws<ArgumentOutOfRangeException>(ConstructFromTooLarge);
            Assert.Throws<ArgumentOutOfRangeException>(ConstructFromNegative);
        }

        void AddressOfIPv6Address()
        {
            Assert.AreEqual(0, IPAddress.IPv6Loopback.Address);
        }

        [Test]
        public void Address()
        {
            Assert.AreEqual(0x0100007F, IPAddress.Loopback.Address);
            Assert.AreEqual(0x0100007F, new IPAddress((long)0x0100007F).Address);

            // https://github.com/fusetools/uno/issues/1509
            if defined (CIL && HOST_OSX)
                Assert.Throws<Exception>(AddressOfIPv6Address);
            else
                Assert.Throws<SocketException>(AddressOfIPv6Address);
        }

        void ParseIPv6AddressWithEmptyScope()
        {
            Assert.AreNotEqual(10, IPAddress.Parse("fe80::a00:27ff:fe84:be2%").ScopeId);
        }

        void ParseIPv6AddressWithScopeTrailingSpace()
        {
            Assert.AreNotEqual(10, IPAddress.Parse("fe80::a00:27ff:fe84:be2%10 ").ScopeId);
        }

        void ParseIPv6AddressWithScopeLetters()
        {
            Assert.AreNotEqual(10, IPAddress.Parse("fe80::a00:27ff:fe84:be2%a").ScopeId);
        }

        void ParseIPv6AddressWithOverlongScope()
        {
            Assert.AreNotEqual(10, IPAddress.Parse("fe80::a00:27ff:fe84:be2%2147483648").ScopeId);
        }

        void ParseIPv6AddressWithVeryOverlongScope()
        {
            Assert.AreNotEqual(10, IPAddress.Parse("fe80::a00:27ff:fe84:be2%10000000000").ScopeId);
        }

        [Test]
        public void Parse()
        {
            Assert.AreEqual(IPAddress.Loopback, IPAddress.Parse("127.0.0.1"));
            Assert.AreEqual(IPAddress.IPv6Loopback, IPAddress.Parse("::1"));
            Assert.AreEqual("fe80::a00:27ff:fe84:be2", IPAddress.Parse("fe80::a00:27ff:fe84:be2").ToString());

            Assert.AreEqual("fe80::a00:27ff:fe84:be2", IPAddress.Parse("fe80::a00:27ff:fe84:be2%0").ToString());
            Assert.AreEqual("fe80::a00:27ff:fe84:be2%1", IPAddress.Parse("fe80::a00:27ff:fe84:be2%1").ToString());
            Assert.AreEqual("fe80::a00:27ff:fe84:be2%10", IPAddress.Parse("fe80::a00:27ff:fe84:be2%10").ToString());

            Assert.AreEqual(10, IPAddress.Parse("fe80::a00:27ff:fe84:be2%10").ScopeId);
            Assert.AreEqual(Int.MaxValue, IPAddress.Parse("fe80::a00:27ff:fe84:be2%2147483647").ScopeId);

            // https://github.com/fusetools/uno/issues/1509
            if defined (!CIL || !HOST_OSX)
            {
                Assert.Throws<FormatException>(ParseIPv6AddressWithEmptyScope);
                Assert.Throws<FormatException>(ParseIPv6AddressWithScopeTrailingSpace);
                Assert.Throws<FormatException>(ParseIPv6AddressWithScopeLetters);
                Assert.Throws<ArgumentOutOfRangeException>(ParseIPv6AddressWithOverlongScope);
                Assert.Throws<FormatException>(ParseIPv6AddressWithVeryOverlongScope);
            }
        }

        [Test]
        public new void ToString()
        {
            Assert.AreEqual("127.0.0.1", IPAddress.Loopback.ToString());
            Assert.AreEqual("::1", IPAddress.IPv6Loopback.ToString());
        }

        [Test]
        public void Equals()
        {
            var a = IPAddress.Parse("1.2.3.4");
            var b = IPAddress.Parse("1.2.3.4");
            Assert.AreEqual(a, b);
            Assert.AreEqual(IPAddress.Parse("127.0.0.1"), IPAddress.Loopback);
        }
    }
}
