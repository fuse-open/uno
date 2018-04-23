using Uno;
using Uno.Testing;

namespace Uno.UX.Tests
{
    public class AttachedPropertyTest
    {
        public class Foo : PropertyObject { }
        public class Bar
        {
            public Property Target { get; set; }

            [UXAttachedPropertySetter("Baz")]
            public static void SetBaz(Foo t, float value)
            {
            }

            [UXAttachedPropertyGetter("Foo")]
            public static float GetBaz(Foo t)
            {
                return 0.0f;
            }
        }

        [Test]
        public void BasicUsage()
        {
            var e = new UXHelpers.AttachedPropertyTest();
            Assert.IsTrue(e.bar.Target != null);
            Assert.IsTrue(e.bar.Target.Object is Foo);
        }
    }
}
