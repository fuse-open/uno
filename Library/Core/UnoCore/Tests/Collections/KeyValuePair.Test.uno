using Uno;
using Uno.Collections;
using Uno.Testing;

namespace Collections.Test
{
    public class KeyValuePairTest
    {
        [Test]
        public void Instantiation00()
        {
            var kvp = new KeyValuePair<int, int>(5, 6);
            Assert.AreEqual(5, kvp.Key);
            Assert.AreEqual(6, kvp.Value);
        }

        [Test]
        public void Instantiation01()
        {
            var kvp = new KeyValuePair<int, string>(27, "string value");
            Assert.AreEqual(27, kvp.Key);
            Assert.AreEqual("string value", kvp.Value);
        }

        [Test]
        public void Instantiation02()
        {
            var kvp = new KeyValuePair<KeyValuePair<string, double>, int>(new KeyValuePair<string, double>("key1 string", 42.0), -8);
            Assert.AreEqual("key1 string", kvp.Key.Key);
            Assert.AreEqual(42.0, kvp.Key.Value);
            Assert.AreEqual(-8, kvp.Value);
        }
    }
}
