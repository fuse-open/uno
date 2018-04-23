using Uno;
using Uno.Testing;
using Uno.Threading;

namespace UnoTest.General
{
    public class StaticInitialization
    {
        class SimpleClass
        {
            public static int X;

            static SimpleClass()
            {
                X = 123;
            }
        }

        [Test]
        public void Simple()
        {
            Assert.AreEqual(123, SimpleClass.X);
        }

        static class SimpleStaticClass
        {
            public static int X;

            static SimpleStaticClass()
            {
                X = 123;
            }
        }

        [Test]
        public void SimpleStatic()
        {
            Assert.AreEqual(123, SimpleStaticClass.X);
        }

        class SimpleInitializerClass
        {
            public static int X = ComputeInt();

            static int ComputeInt()
            {
                return 123;
            }
        }

        [Test]
        public void SimpleInitializer()
        {
            Assert.AreEqual(123, SimpleInitializerClass.X);
        }

        static class SimpleInitializerStaticClass
        {
            public static int X = ComputeInt();

            static int ComputeInt()
            {
                return 123;
            }
        }

        [Test]
        public void SimpleInitializerStatic()
        {
            Assert.AreEqual(123, SimpleInitializerStaticClass.X);
        }

        class CircularInitializerClass
        {
            public static int Before = BeforeXInit();
            public static int X = ComputeX();
            public static int Y = ComputeY();

            static int BeforeXInit()
            {
                return X;
            }

            static int ComputeX()
            {
                return 123;
            }

            static int ComputeY()
            {
                return X;
            }
        }

        [Test]
        public void CircularInitializer()
        {
            Assert.AreEqual(0, CircularInitializerClass.Before);
            Assert.AreEqual(123, CircularInitializerClass.Y);
        }

        class StaticCircularInitializerClass
        {
            public static int Before = BeforeXInit();
            public static int X = ComputeX();
            public static int Y = ComputeY();

            static int BeforeXInit()
            {
                return X;
            }

            static int ComputeX()
            {
                return 123;
            }

            static int ComputeY()
            {
                return X;
            }
        }

        [Test]
        public void StaticCircularInitializer()
        {
            Assert.AreEqual(0, StaticCircularInitializerClass.Before);
            Assert.AreEqual(123, StaticCircularInitializerClass.Y);
        }

        class CircularInitializerClass1
        {
            public static int Before = BeforeXInit();
            public static int X = ComputeX();
            public static int Y = ComputeY();

            static int BeforeXInit()
            {
                return X;
            }

            static int ComputeX()
            {
                return 123;
            }

            static int ComputeY()
            {
                return CircularInitializerClass2.X;
            }
        }

        class CircularInitializerClass2
        {
            public static int Before = BeforeXInit();
            public static int X = ComputeX();

            static int BeforeXInit()
            {
                return X;
            }

            static int ComputeX()
            {
                return CircularInitializerClass1.X;
            }
        }

        [Test]
        public void TwoCircularInitializerClasses()
        {
            Assert.AreEqual(0, CircularInitializerClass1.Before);
            Assert.AreEqual(123, CircularInitializerClass1.X);
            Assert.AreEqual(123, CircularInitializerClass1.Y);
            Assert.AreEqual(0, CircularInitializerClass2.Before);
            Assert.AreEqual(123, CircularInitializerClass2.X);
        }

        static class StaticCircularInitializerClass1
        {
            public static int Before = BeforeXInit();
            public static int X = ComputeX();
            public static int Y = ComputeY();

            static int BeforeXInit()
            {
                return X;
            }

            static int ComputeX()
            {
                return 123;
            }

            static int ComputeY()
            {
                return CircularInitializerClass2.X;
            }
        }

        static class StaticCircularInitializerClass2
        {
            public static int Before = BeforeXInit();
            public static int X = ComputeX();

            static int BeforeXInit()
            {
                return X;
            }

            static int ComputeX()
            {
                return CircularInitializerClass1.X;
            }
        }

        [Test]
        public void TwoStaticCircularInitializerClasses()
        {
            Assert.AreEqual(0, StaticCircularInitializerClass1.Before);
            Assert.AreEqual(123, StaticCircularInitializerClass1.X);
            Assert.AreEqual(123, StaticCircularInitializerClass1.Y);
            Assert.AreEqual(0, StaticCircularInitializerClass2.Before);
            Assert.AreEqual(123, StaticCircularInitializerClass2.X);
        }

        static class ThreadStarter
        {
            public static void RunThread()
            {
                var x = ThreadedCircularInitializerClass2.X;
            }
        }

        class ThreadedCircularInitializerClass1
        {
            public static int S = Sleep();
            public static int X = Compute();

            static int Sleep()
            {
                var t = new Thread(ThreadStarter.RunThread);
                t.Start();

                Thread.Sleep(250);

                return 123;
            }

            static int Compute()
            {
                var result = ThreadedCircularInitializerClass2.X;
                return 123;
            }
        }

        class ThreadedCircularInitializerClass2
        {
            public static int S = Sleep();
            public static int X = Compute();

            static int Sleep()
            {
                Thread.Sleep(500);

                return 0;
            }

            static int Compute()
            {
                var result = ThreadedCircularInitializerClass1.X;
                return result;
            }
        }

        [Test]
        public void TwoThreadedCircularInitializerClasses()
        {
            Assert.AreEqual(123, ThreadedCircularInitializerClass1.X);
        }

        class ThrowsInStaticInitializerClass
        {
            static ThrowsInStaticInitializerClass()
            {
                throw new Exception("inner");
            }
        }

        class ThrowsInStaticInitializerDerivedClass : ThrowsInStaticInitializerClass
        {
        }

        [Test]
        public void ThrowsInStaticInitializer()
        {
            Assert.Throws<TypeInitializationException>(() => new ThrowsInStaticInitializerClass());
            Assert.Throws<TypeInitializationException>(() => new ThrowsInStaticInitializerClass());

            Assert.Throws<TypeInitializationException>(() => {
                try
                {
                    new ThrowsInStaticInitializerDerivedClass();
                }
                catch (TypeInitializationException tie)
                {
                    Assert.Contains("The type initializer for", tie.Message);
                    Assert.Contains("ThrowsInStaticInitializerClass", tie.Message);
                    Assert.Contains("threw an exception.", tie.Message);

                    Assert.AreNotEqual(null, tie.InnerException);
                    Assert.AreEqual("inner", tie.InnerException.Message);

                    Assert.AreEqual(null, tie.InnerException.InnerException);
                    throw;
                }
            });
        }
    }
}
