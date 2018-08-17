using Uno;
using Uno.Testing;
using Uno.Net.Http;
using Uno.Collections;

namespace Http.Test
{
    public class UriTests
    {
        [Test]
        public void EscapeDataString()
        {
            Assert.AreEqual("test%3Fx%3D1%203%26y%3D5%2B7", Uri.EscapeDataString("test?x=1 3&y=5+7"));
            Assert.AreEqual("wh%20ite%3Ds%20%09%20p%20%0Aa%20%7Cbces%20%0D", Uri.EscapeDataString("wh ite=s \t p \na |bces \r"));
            Assert.AreEqual("%C2%B1%E0%A5%90", Uri.EscapeDataString("\u00B1\u0950"));
            Assert.AreEqual("foo%2520bar", Uri.EscapeDataString("foo%20bar"));
            Assert.AreEqual("foo%23bar", Uri.EscapeDataString("foo#bar"));
            Assert.AreEqual("%3B-%2F-%3F-%3A-%40-%26-%3D-%2B-%24-%2C", Uri.EscapeDataString(";-/-?-:-@-&-=-+-$-,")); // RFC2396 Reserved Characters
            Assert.AreEqual("-_.%21~%2A%27%28%29", Uri.EscapeDataString("-_.!~*'()")); // RFC2396 Unreserved Characters
        }

        [Test]
        public void UnescapeDataString()
        {
            Assert.AreEqual("test?x=1 3&y=5+7", Uri.UnescapeDataString("test%3fx%3d1%203%26y%3d5%2b7"));
            Assert.AreEqual("wh ite=s \t p \na |bces \r", Uri.UnescapeDataString("wh%20ite%3ds%20%09%20p%20%0aa%20%7cbces%20%0d"));
            Assert.AreEqual("\u00B1\u0950", Uri.UnescapeDataString("%c2%b1%e0%a5%90"));
        }

        [Test, Obsolete]
        public void Combine()
        {
            Assert.AreEqual("http://test.com/test/uri", Uri.Combine("http://test.com", "test/uri"));
            Assert.AreEqual("http://test.com/test/uri", Uri.Combine("http://test.com/test", "/uri"));
            Assert.AreEqual("http://test.com/test/uri", Uri.Combine("http://test.com/", "/test/uri"));
        }

        [Test]
        public void AbsoluteUri()
        {
            Assert.AreEqual("http://test.com/", new Uri("http://test.com").AbsoluteUri);
            Assert.AreEqual("http://test.com/", new Uri(" http://test.com").AbsoluteUri);
            Assert.AreEqual("http://test.com/", new Uri("http://test.com ").AbsoluteUri);
            Assert.AreEqual("http://username:password@test.com:1337/test?foo=bar#baz", new Uri("http://username:password@test.com:1337/test?foo=bar#baz").AbsoluteUri);
            Assert.AreEqual("mailto:foo@test.com", new Uri("mailto:foo@test.com").AbsoluteUri);
            Assert.AreEqual("mailto://test.com", new Uri("mailto://test.com").AbsoluteUri);
            Assert.AreEqual("tel:12345678", new Uri("tel:12345678").AbsoluteUri);
        }

        [Test]
        public void OriginalString()
        {
            Assert.AreEqual("http://test.com", new Uri("http://test.com").OriginalString);
            Assert.AreEqual(" http://test.com", new Uri(" http://test.com").OriginalString);
            Assert.AreEqual("http://test.com ", new Uri("http://test.com ").OriginalString);
        }

        [Test]
        public void Scheme()
        {
            Assert.AreEqual("http", new Uri("http://test.com").Scheme);
            Assert.AreEqual("http", new Uri("HTTP://test.com").Scheme);

            Assert.AreEqual("ftp", new Uri("ftp://test.com").Scheme);
            Assert.AreEqual("telnet", new Uri("telnet://test.com").Scheme);
            Assert.AreEqual("mailto", new Uri("mailto:some.name@domain.com").Scheme);
            Assert.AreEqual("fb", new Uri("fb://profile/of/someone").Scheme);
            Assert.AreEqual("whatever", new Uri("whatever://").Scheme);
            Assert.AreEqual("whatever", new Uri("WHATEVER://").Scheme);

            Assert.AreEqual("test", new Uri("test://whatevertest").Scheme);
            Assert.AreEqual("test", new Uri("test://tested.testing.test").Scheme);
            Assert.AreEqual("test", new Uri("test://tested.testing.test/test/in/the?testing=framework&with=the#console").Scheme);
        }

        [Test]
        public void UserInfo()
        {
            Assert.AreEqual("username:password", new Uri("http://username:password@test.com").UserInfo);
            Assert.AreEqual("username", new Uri("http://username@test.com").UserInfo);
            Assert.IsTrue(string.IsNullOrEmpty(new Uri("http://test.com").UserInfo));
            Assert.AreEqual("some.name", new Uri("mailto:some.name@domain.com").UserInfo);

            Assert.AreEqual("", new Uri("http://@test.com").UserInfo);
            Assert.AreEqual(":password", new Uri("http://:password@test.com").UserInfo);
            Assert.AreEqual("user:", new Uri("http://user:@test.com").UserInfo);

            Assert.AreEqual("username:password", new Uri("mailto:username:password@test.com").UserInfo);
        }

        [Test]
        public void Host()
        {
            Assert.AreEqual("test.com", new Uri("http://test.com").Host);
            Assert.AreEqual("12.35.49.49", new Uri("http://12.35.49.49").Host);
            Assert.AreEqual("test.com", new Uri("mailto:username@test.com").Host);
        }

        [Test]
        public void Port()
        {
            Assert.AreEqual(80, new Uri("http://test.com").Port); //default port
            Assert.AreEqual(7878, new Uri("http://12.35.49.49:7878").Port);
            Assert.AreEqual(21, new Uri("ftp://12.35.49.49").Port); //default port
            Assert.AreEqual(455, new Uri("ftp://test.com:455").Port);
            Assert.AreEqual(25, new Uri("mailto:username@test.com").Port);
        }

        [Test]
        public void Authority()
        {
            Assert.AreEqual("test.com", new Uri("http://test.com").Authority);
            Assert.AreEqual("12.35.49.49", new Uri("ftp://12.35.49.49").Authority);
            Assert.AreEqual("test.com:455", new Uri("ftp://test.com:455").Authority);
            Assert.AreEqual("12.35.49.49:7878", new Uri("http://12.35.49.49:7878").Authority);
            Assert.AreEqual("test.com", new Uri("mailto:username@test.com").Authority);
        }

        [Test]
        public void Segments()
        {
            Assert.AreEqual(new string[] { "/" }, new Uri("http://test.com").Segments);
            Assert.AreEqual(new string[] { "/" }, new Uri("http://test.com/").Segments);
            Assert.AreEqual(new string[] { "/", "/" }, new Uri("http://test.com//").Segments);
            Assert.AreEqual(new string[] { "/", "/", "/" }, new Uri("http://test.com///").Segments);
            Assert.AreEqual(new string[] { "/", "foo" }, new Uri("http://test.com/foo").Segments);
            Assert.AreEqual(new string[] { "/", "foo/" }, new Uri("http://test.com/foo/").Segments);
            Assert.AreEqual(new string[] { "/", "foo/", "bar" }, new Uri("http://test.com/foo/bar").Segments);
            Assert.AreEqual(new string[] { "/", "foo/", "bar" }, new Uri("http://test.com/foo/bar?baz").Segments);
            Assert.AreEqual(new string[] { "/", "foo/", "bar" }, new Uri("http://test.com/foo/bar#baz").Segments);

            Assert.AreEqual(new string[] { "/", "foo;/", ":@&=+$,bar" }, new Uri("http://test.com/foo;/:@&=+$,bar").Segments); // RFC2396 Reserved Characters (except '?')
            Assert.AreEqual(new string[] { "/", "foo-_.!~*'()bar" }, new Uri("http://test.com/foo-_.!~*'()bar").Segments); // RFC2396 Unreserved Characters

            Assert.AreEqual(new string[] { "/", "foo;/", "%3F:@&=+$,bar" }, new Uri("ftp://test.com/foo;/?:@&=+$,bar").Segments); // RFC2396 Reserved Characters
            Assert.AreEqual(new string[] { "/", "foo-_.!~*'()bar" }, new Uri("ftp://test.com/foo-_.!~*'()bar").Segments); // RFC2396 Unreserved Characters

            Assert.AreEqual(new string[] { "/", "foo%20bar" }, new Uri("http://test.com/foo bar").Segments);
            Assert.AreEqual(new string[] { "/", "foo%20bar" }, new Uri("http://test.com/foo%20bar").Segments);
            Assert.AreEqual(new string[] { "/", "foo%C2%B1%E0%A5%90bar" }, new Uri("http://test.com/foo\u00B1\u0950bar").Segments);
            Assert.AreEqual(new string[] { "/", "foo%01bar" }, new Uri("http://test.com/foo\u0001bar").Segments);
            Assert.AreEqual(new string[] { "/", "foo%60bar" }, new Uri("http://test.com/foo\u0060bar").Segments);
            Assert.AreEqual(new string[] { "/", "foo%7Fbar" }, new Uri("http://test.com/foo\u007Fbar").Segments);

            Assert.AreEqual(new string[] { }, new Uri("mailto:username@test.com").Segments);
            Assert.AreEqual(new string[] { "/", "foo/", "bar" }, new Uri("mailto:username@test.com/foo/bar").Segments);
            Assert.AreEqual(new string[] { "12345678" }, new Uri("tel:12345678").Segments);
        }

        [Test]
        public void AbsolutePath()
        {
            Assert.AreEqual("/", new Uri("http://username:password@test.com").AbsolutePath);
            Assert.AreEqual("/", new Uri("http://username:password@test.com/").AbsolutePath);
            Assert.AreEqual("/somecontroller/index", new Uri("http://username:password@test.com/somecontroller/index").AbsolutePath);
            Assert.AreEqual("/somecontroller/index", new Uri("http://username:password@test.com/somecontroller/index?test=123").AbsolutePath);
            Assert.AreEqual("/somecontroller/index", new Uri("http://username:password@test.com/somecontroller/index#test=123").AbsolutePath);
            Assert.AreEqual("/somecontroller/index%3Ftest=123", new Uri("ftp://username:password@test.com/somecontroller/index?test=123").AbsolutePath);
            Assert.AreEqual("/somecontroller/index%3Ftest=123", new Uri("ftp://username:password@test.com/somecontroller/index?test=123#action").AbsolutePath);
            Assert.AreEqual("/foo", new Uri("http://test.com/foo#bar/baz").AbsolutePath);
            Assert.AreEqual("/foo%bar/baz", new Uri("http://test.com/foo%bar/baz").AbsolutePath);

            Assert.AreEqual("", new Uri("mailto:username@test.com").AbsolutePath);
        }

        [Test]
        public void Query()
        {
            Assert.IsTrue(string.IsNullOrEmpty(new Uri("http://username:password@test.com").Query));
            Assert.IsTrue(string.IsNullOrEmpty(new Uri("http://username:password@test.com/somecontroller/index#test=123").Query));
            Assert.IsTrue(string.IsNullOrEmpty(new Uri("ftp://username:password@test.com/somecontroller/index?test=123").Query));
            Assert.AreEqual("?test=123", new Uri("http://username:password@test.com/somecontroller/index?test=123#action").Query);
        }

        [Test]
        public void Fragment()
        {
            Assert.IsTrue(string.IsNullOrEmpty(new Uri("http://username:password@test.com").Fragment));
            Assert.IsTrue(string.IsNullOrEmpty(new Uri("http://username:password@test.com/somecontroller/index?test=123").Fragment));
            Assert.AreEqual("#test=123", new Uri("ftp://username:password@test.com/somecontroller/index#test=123").Fragment);
            Assert.AreEqual("#action", new Uri("http://username:password@test.com/somecontroller/index?test=123#action").Fragment);
            Assert.AreEqual("#action?test=123", new Uri("http://username:password@test.com/somecontroller/index#action?test=123").Fragment);
            Assert.AreEqual("#foo", new Uri("mailto:test.com#foo").Fragment);
        }

        [Test]
        public void PathAndQuery()
        {
            Assert.AreEqual("/", new Uri("http://username:password@test.com").PathAndQuery);
            Assert.AreEqual("/somecontroller/index?test=123", new Uri("http://username:password@test.com/somecontroller/index?test=123").PathAndQuery);
            Assert.AreEqual("/somecontroller/index?test=123", new Uri("http://username:password@test.com/somecontroller/index?test=123#action").PathAndQuery);
            Assert.AreEqual("/somecontroller/index", new Uri("http://username:password@test.com/somecontroller/index#action?test=123").PathAndQuery);
        }

        [Test, Obsolete]
        public void GetQueryParameters()
        {
            var uri = new Uri("http://username:password@test.com/somecontroller/index?a=1&b=2&cdef=test#action");
            var parameters = uri.GetQueryParameters();
            Assert.AreEqual(3, parameters.Keys.Count);

            CheckValue(parameters, "a", "1");
            CheckValue(parameters, "cdef", "test");

            uri = new Uri("http://username:password@test.com/somecontroller/index#action");
            parameters = uri.GetQueryParameters();
            Assert.AreEqual(0, parameters.Keys.Count);
        }

        [Test]
        public void CreateFileUriWithAbsolutePath()
        {
            var uri = new Uri("file:///some-file.txt");
            Assert.AreEqual(uri.Scheme, "file");
            Assert.AreEqual(uri.AbsolutePath, "/some-file.txt");
        }

        [Test]
        public void InvalidScheme()
        {
            Assert.Throws<UriFormatException>(() => new Uri("://test.com"));
        }

        [Test]
        public void EmptyUri()
        {
            Assert.Throws<ArgumentNullException>(() => new Uri(null));
            Assert.Throws<UriFormatException>(() => new Uri(string.Empty));
        }

        [Test]
        public void InvalidUserInfo()
        {
            Assert.Throws<UriFormatException>(() => new Uri("http://us\\er:password@test.com"));
        }

        [Test]
        public void InvalidHostInfo()
        {
            Assert.Throws<UriFormatException>(() => new Uri("http://.test"));
            Assert.Throws<UriFormatException>(() => new Uri("http://test..com"));
            Assert.Throws<UriFormatException>(() => new Uri("http://:1234"));
            Assert.Throws<UriFormatException>(() => new Uri("http://{test}.com"));
            Assert.Throws<UriFormatException>(() => new Uri("http://test.com:port"));
            Assert.Throws<UriFormatException>(() => new Uri("http://test.com:15:15"));
        }

        [Test]
        public void InvalidFormat()
        {
            Assert.Throws<UriFormatException>(() => new Uri("http://"));
            Assert.Throws<UriFormatException>(() => new Uri("ftp:test.com"));
            Assert.Throws<UriFormatException>(() => new Uri("www.test.com"));
            Assert.Throws<UriFormatException>(() => new Uri("ftp//test.com"));
        }

        private void CheckValue(IDictionary<string, string> dictionary, string key, string value)
        {
            Assert.IsTrue(dictionary.ContainsKey(key));
            Assert.AreEqual(value, dictionary[key]);
        }
    }
}
