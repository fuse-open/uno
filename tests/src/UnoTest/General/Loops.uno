using Uno;
using Uno.Testing;

namespace UnoTest.General
{
    public class Loops
    {
        static bool foo = true;

        public static void EmptyMethod()
        {
            int i;
            for (;;) {}
        }

        public void Run()
        {
            while (true)
            {
                Assert.IsTrue(true);
                if (foo) break;
                Assert.IsTrue(false);
            }

            int e = 0;
            while (e < 6) e++;
            Assert.AreEqual(6, e);
            while (true)
            {
                e--;
                if (e == 0) break;
            }
            Assert.AreEqual(0, e);

            while (false) ;

            int count = 0;
            for (int i = 0; i < 10; i++) count++;
            Assert.AreEqual(10, count);

            int count2 = 0;
            for ( ; count > 0; count--) count2++;
            Assert.AreEqual(0, count);
            Assert.AreEqual(10, count2);

            int count3;
            for (count3 = 0; ; count3++)
            {
                if (count3 >= 10) break;
            }
            Assert.AreEqual(10, count3);

            int count4;
            for (count4 = 0; count4 < 8; ) count4++;
            Assert.AreEqual(8, count4);

            int count5;
            for (count5 = 0; ; )
            {
                if (count5 >= 4) break;
                count5++;
            }
            Assert.AreEqual(4, count5);

            int count6 = 0;
            for ( ; count6 < 7; ) count6++;
            Assert.AreEqual(7, count6);

            int count7 = 0;
            for( ; ; count7++)
            {
                if (count7 >= 12) break;
            }
            Assert.AreEqual(12, count7);

            for ( ; ; ) break;
            for ( ; false; ) ;
            do ; while (false);
            do { } while (false);

            int count8 = 0;
            do { count8++; } while (false);
            Assert.AreEqual(1, count8);

            count8 = 0;
            do count8--; while (false);
            Assert.AreEqual(-1, count8);

            for (int i = 0, l = 1; i < 0; i++)
                continue;

            int intVar;
            long longVar;
            ulong ulongVar;
            for (intVar = 0, longVar = 1, ulongVar = 2; intVar < longVar; intVar++)
                ;

            int idx = 2;
            for (int loop = 0; ( loop < 2 ) && ( idx > 0 ); ++loop, --idx)
                ;
            Assert.IsTrue(idx < 2);
        }
    }
}
