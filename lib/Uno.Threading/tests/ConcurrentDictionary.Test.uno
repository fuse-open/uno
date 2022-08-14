using Uno;
using Uno.Collections;
using Uno.Threading;
using Uno.Testing;

namespace ThreadingTests
{
    public class ConcurrentDictionaryTest
    {
        [Test]
        public void Basic()
        {
            var dict = new ConcurrentDictionary<string, string>();
            ((IDictionary<string, string>)dict).Add("foo", "bar");

            Assert.AreEqual(1, ((IDictionary<string, string>)dict).Keys.Count);
            Assert.IsTrue(((IDictionary<string, string>)dict).Keys.Contains("foo"));
            Assert.IsFalse(((IDictionary<string, string>)dict).Keys.Contains("bar"));

            Assert.AreEqual(1, ((IDictionary<string, string>)dict).Values.Count);
            Assert.IsFalse(((IDictionary<string, string>)dict).Values.Contains("foo"));
            Assert.IsTrue(((IDictionary<string, string>)dict).Values.Contains("bar"));

            dict.Clear();

            Assert.AreEqual(0, ((IDictionary<string, string>)dict).Keys.Count);
            Assert.IsFalse(((IDictionary<string, string>)dict).Keys.Contains("foo"));

            Assert.AreEqual(0, ((IDictionary<string, string>)dict).Values.Count);
            Assert.IsFalse(((IDictionary<string, string>)dict).Values.Contains("bar"));
        }
    }
}
