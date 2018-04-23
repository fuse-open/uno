using Uno;
using Uno.IO;
using Uno.Testing;
using Uno.Net.Sockets;

namespace Uno.Net.Sockets.Test
{
    public class DnsTest
    {
        [Test]
        public void GetHostAddressesIPv4()
        {
            var addresses = Dns.GetHostAddresses("google-public-dns-b.google.com");
            Assert.Contains(IPAddress.Parse("8.8.4.4"), addresses);
        }

        [Test]
        [Ignore("Requires IPv6 internet connectivity, and Uno.Testing cannot skip tests at run-time")]
        public void GetHostAddressesIPv6()
        {
            var addresses = Dns.GetHostAddresses("google-public-dns-b.google.com");
            Assert.Contains(IPAddress.Parse("2001:4860:4860::8844"), addresses);
        }

        void GetHostAddressesNull()
        {
            Assert.AreEqual(0, Dns.GetHostAddresses(null).Length);
        }

        void GetHostAddressesInvalidDomain()
        {
            Assert.AreEqual(0, Dns.GetHostAddresses("$").Length);
        }

        void GetHostAddressesLongHostname()
        {
            // we don't currently have String(char, int), so this hack emulates it:
            var hostname = String.Empty.PadRight(254, 'c');
            Assert.AreEqual(0, Dns.GetHostAddresses(hostname).Length);
        }

        void GetHostAddressesLongHostname2()
        {
            // we don't currently have String(char, int), so this hack emulates it:
            var hostname = String.Empty.PadRight(254, 'c').PadRight('.');
            Assert.AreEqual(0, Dns.GetHostAddresses(hostname).Length);
        }

        void GetHostAddressesLongHostname3()
        {
            // we don't currently have String(char, int), so this hack emulates it:
            var hostname = String.Empty.PadRight(255, 'c');
            Assert.AreEqual(0, Dns.GetHostAddresses(hostname).Length);
        }

        void GetHostAddressesLongHostname4()
        {
            // we don't currently have String(char, int), so this hack emulates it:
            var hostname = String.Empty.PadRight(256, 'c');
            Assert.AreEqual(0, Dns.GetHostAddresses(hostname).Length);
        }

        [Test]
        public void GetHostAddresses()
        {
            Assert.AreEqual(IPAddress.Loopback, Dns.GetHostAddresses(IPAddress.Loopback.ToString())[0]);

            Assert.Throws<ArgumentNullException>(GetHostAddressesNull);
            Assert.Throws<SocketException>(GetHostAddressesInvalidDomain);
            Assert.Throws<SocketException>(GetHostAddressesLongHostname);
            Assert.Throws<SocketException>(GetHostAddressesLongHostname2);

            // https://github.com/fusetools/uno/issues/1518
            if defined (CIL && HOST_OSX)
            {
                Assert.Throws<SocketException>(GetHostAddressesLongHostname3);
                Assert.Throws<SocketException>(GetHostAddressesLongHostname4);
            }
            else
            {
                Assert.Throws<ArgumentOutOfRangeException>(GetHostAddressesLongHostname3);
                Assert.Throws<ArgumentOutOfRangeException>(GetHostAddressesLongHostname4);
            }
        }

        [Test]
        public void GetHostAddressesEmpty()
        {
            Assert.AreNotEqual(0, Dns.GetHostAddresses(string.Empty).Length);
        }
    }
}
