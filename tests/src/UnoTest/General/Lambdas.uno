using Uno;
using Uno.Collections;
using Uno.Diagnostics;
using Uno.IO;
using Uno.Testing;

namespace UnoTest.General
{
    public class Lambdas
    {
        [Test]
        public void Run()
        {
            int count = 0;
            Action a = () => count++;
            a();
            Assert.AreEqual(1, count);

            float sum = 5.2f;
            Action<float> b = x => sum += x;
            b(2.1f);
            Assert.AreEqual(7.3f, sum);

            Func<int, int, bool> c = (x, y) => x == y;
            Assert.IsTrue(c(5, 5));

            count = 0;
            sum = 7.4f;

            Action<float> action3 = x => {
                count++;
                sum += x;
            };
            action3(2.9f);
            Assert.AreEqual(10.3f, sum);

            Func<int, int, int> action4 = (x, y) => {
                count++;
                return x+y;
            };
            Assert.AreEqual(10, action4(8, 2));
            Assert.AreEqual(2, count);
        }

        [Test]
        public void CaptureThis()
        {
            var c = new CaptureThisClass();
            c.Run1();
            Assert.AreEqual(124, c.X);
            c.Run2();
            Assert.AreEqual(669, c.X);
        }

        class CaptureThisClass
        {
            public int X;

            public void Run1()
            {
                X = 123;
                Action f = () => ++X;
                f();
            }

            public void Run2()
            {
                int a = 123;
                X = 223;
                Action<int> f = b => X += a + b;
                f(323);
            }
        }

        [Test]
        public void NoFreeVars()
        {
            Func<int, int, int> f = (a, b) => a + b;

            Assert.AreEqual(43, f(41, 2));
        }

        [Test]
        public void NestedLambdas()
        {
            var x = 1;
            var y = 2;

            Func<Func<int>> f = () =>
            {
                x = 2;
                y = 3;
                Func<int> result = () =>
                {
                    ++y;
                    return x + y;
                };
                return result;
            };

            var g = f();
            Assert.AreEqual(6, g());
            Assert.AreEqual(7, g());
            f();
            Assert.AreEqual(6, g());
        }

        [Test]
        public void NestedLambdasWithoutFreeVars()
        {
            Func<Func<int>> f = () => () => 43;
            Assert.AreEqual(43, f()());
        }

        [Test]
        public void NestedLambdasWithoutFreeVars2()
        {
            Func<Func<int>> f = () => { return () => { return 43; }; };
            Assert.AreEqual(43, f()());
        }

        [Test]
        public void ExplicitParameterType()
        {
            Action<object> f = (object o) => { };
        }

        int _x;

        [Test]
        public void NestedLambdasCapturingThis()
        {
            _x = 123;

            Func<Func<int>> f = () =>
            {
                _x = 0;
                return () =>
                {
                    ++_x;
                    return _x;
                };
            };

            var g = f();
            Assert.AreEqual(0, _x);
            var x = g();
            Assert.AreEqual(1, x);
            Assert.AreEqual(1, _x);
            x = g();
            Assert.AreEqual(2, x);
            Assert.AreEqual(2, _x);
        }

        // To avoid optimising the ifs away
        bool ReturnTrue()
        {
            return true;
        }

        [Test]
        public void DifferentScopes()
        {
            List<int> outer = new List<int>();
            Action<int> f = null;
            Action<int> g = null;

            if (ReturnTrue())
            {
                int inner1 = 0;
                f = (x) =>
                {
                    inner1 += x;
                    outer.Add(inner1);
                };
            }

            if (ReturnTrue())
            {
                int inner2 = 0;
                g = (x) =>
                {
                    inner2 += x;
                    outer.Add(inner2);
                };
            }

            f(2);
            f(2);
            Assert.AreCollectionsEqual(new [] { 2, 4 }, outer);
            g(3);
            g(3);
            Assert.AreCollectionsEqual(new [] { 2, 4, 3, 6 }, outer);
        }

        List<int> _outerList;

        [Test]
        public void DifferentScopesCapturingThis()
        {
            _outerList = new List<int>();
            Action<int> f = null;
            Action<int> g = null;

            if (ReturnTrue())
            {
                while (ReturnTrue())
                {
                    int inner1 = 0;
                    f = (x) =>
                    {
                        inner1 += x;
                        _outerList.Add(inner1);
                    };
                    break;
                }
            }

            if (ReturnTrue())
            {
                while (ReturnTrue())
                {
                    int inner2 = 0;
                    g = (x) =>
                    {
                        inner2 += x;
                        _outerList.Add(inner2);
                    };
                    break;
                }
            }

            f(2);
            f(2);
            Assert.AreCollectionsEqual(new [] { 2, 4 }, _outerList);
            g(3);
            g(3);
            Assert.AreCollectionsEqual(new [] { 2, 4, 3, 6 }, _outerList);
        }

        [Test]
        public void DeepDifferentScopes()
        {
            var outerList = new List<int>();
            Action<int> f = null;
            Action<int> g = null;

            if (ReturnTrue())
            {
                while (ReturnTrue())
                {
                    int self = 1;
                    f = (x) =>
                    {
                        outerList.Add(self);
                        self = x;
                    };
                    break;
                }
            }

            if (ReturnTrue())
            {
                while (ReturnTrue())
                {
                    int self2 = 2;
                    g = (x) =>
                    {
                        outerList.Add(self2);
                        self2 = x;
                    };
                    break;
                }
            }

            f(3);
            f(4);
            Assert.AreCollectionsEqual(new [] { 1, 3 }, outerList);
            g(5);
            g(6);
            Assert.AreCollectionsEqual(new [] { 1, 3, 2, 5 }, outerList);
        }

        [Test]
        public void DifferentScopesCapturingThisNameClashes()
        {
            _outerList = new List<int>();
            Action<int> f = null;
            Action<int> g = null;

            if (ReturnTrue())
            {
                while (ReturnTrue())
                {
                    int self = 0;
                    f = (x) =>
                    {
                        self += x;
                        _outerList.Add(self);
                    };
                    break;
                }
            }

            if (ReturnTrue())
            {
                while (ReturnTrue())
                {
                    int parent = 0;
                    g = (x) =>
                    {
                        parent += x;
                        _outerList.Add(parent);
                    };
                    break;
                }
            }

            f(2);
            f(2);
            Assert.AreCollectionsEqual(new [] { 2, 4 }, _outerList);
            g(3);
            g(3);
            Assert.AreCollectionsEqual(new [] { 2, 4, 3, 6 }, _outerList);
        }


        [Test]
        public void DifferentScopesWithNoFreeVars()
        {
            Func<int, int> f = null;
            Func<int, int> g = null;

            if (ReturnTrue())
            {
                try
                {
                    f = (x) =>
                    {
                        return x + x;
                    };
                }
                catch (Exception e)
                {
                }
            }

            if (ReturnTrue())
            {
                try
                {
                    int inner2 = 0;
                    g = (x) =>
                    {
                        return x * x;
                    };
                }
                catch (Exception e)
                {
                }
            }

            Assert.AreEqual(2, f(1));
            Assert.AreEqual(6, f(3));
            Assert.AreEqual(1, g(1));
            Assert.AreEqual(9, g(3));
        }

        [Test]
        public void ParameterCapture()
        {
            Assert.AreEqual("41hello4143", ParameterCaptureHelper(41, "hello")(43));
        }

        Func<int, string> ParameterCaptureHelper(int param1, string param2)
        {
            return (x) =>
            {
                return param1 + param2 + param1 + x;
            };
        }

        [Test]
        public void ParameterMutation()
        {
            ParameterMutationHelper(123, "hello");
        }

        void ParameterMutationHelper(int i, string s)
        {
            Func<int> f = null;
            Func<string> g = null;

            if (ReturnTrue())
            {
                int inner = 0;
                f = () =>
                {
                    ++i;
                    ++inner;
                    return inner + i;
                };
            }
            if (ReturnTrue())
            {
                string inner = "lol";
                g = () =>
                {
                    s = s + "hello";
                    inner = inner + "lol";
                    return s + inner;
                };
            }

            Assert.AreEqual(123, i);
            Assert.AreEqual(125, f());
            Assert.AreEqual(124, i);
            Assert.AreEqual(127, f());
            Assert.AreEqual(125, i);

            Assert.AreEqual("hello", s);
            Assert.AreEqual("hellohellolollol", g());
            Assert.AreEqual("hellohello", s);
            Assert.AreEqual("hellohellohellolollollol", g());
            Assert.AreEqual("hellohellohello", s);
        }

        [Test]
        public void CaptureTryCatchException()
        {
            Func<Exception> f = null;

            try
            {
                throw new IOException("Yo!");
            }
            catch (IOException e)
            {
                f = () => e;
            }
            catch (Exception e)
            {
                f = () => e;
            }

            Assert.AreEqual("Yo!", f().Message);
        }
    }

    public class LambdasClashClass
    {
        class NamesClashingWithGeneratedNames_closure1
        {
            public int X = 123;
        }

        [Test]
        public void NamesClashingWithGeneratedNames()
        {
            var x = 123;

            Action f = () => ++x;

            var generated_closure2 = "hello";

            Assert.AreEqual("hello", generated_closure2);

            var c = new NamesClashingWithGeneratedNames_closure1();
            Assert.AreEqual(123, c.X);
        }
    }
}
