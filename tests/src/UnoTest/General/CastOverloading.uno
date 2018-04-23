using Uno;
using Uno.Testing;

namespace UnoTest.General
{
    public class CastOverloading
    {
        class class1
        {
            public int val;
            public class1()
            {
                val=0;
            }
            public class1(int aVal)
            {
                this.val = aVal;
            }
            public class1(float aVal)
            {
                this.val = (int)aVal;
            }
            public class1(class2 aVal)
            {
                this.val = (int)aVal.cha;
            }
            public static implicit operator class1(int theVal)
            {
                return new class1(theVal);
            }
            public static implicit operator class1(float theVal)
            {
                return new class1(theVal);
            }
            public static implicit operator class1(class2 theVal)
            {
                return new class1(theVal);
            }
            public static explicit operator int(class1 a)
            {
                return a.val;
            }
            public static explicit operator class2(class1 a)
            {
                return new class2(a.val);
            }
        }

        class class2
        {
            public char cha;
            public class2()
            {
                cha = 'A';
            }
            public class2(int val)
            {
                cha = (char)val;
            }
        }

        [Test]
        public void Run()
        {
            var der = new class1(5);
            int ber = 0;
            var her = (class1)ber;
            Assert.AreEqual(0, her.val);
            ber = (int)der;
            Assert.AreEqual(5, ber);
            float a = 5.5f;
            der = (class1)a;
            Assert.AreEqual(5, der.val);

            var aa = new class2();
            var bb = new class1(66);
            bb = (class1)aa;
            Assert.AreEqual(65, bb.val);

            bb.val = 66;
            aa = (class2)bb;
            Assert.AreEqual('B', aa.cha);
        }
    }
}
