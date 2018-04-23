using Uno;
using Uno.Collections;
using Uno.Testing;

namespace UnoTest.General
{
    namespace Recursive
    {
        struct TestStruct<T>
        {
            public static TestStruct<T> X = new TestStruct<T>();
            int _dummy;
        }

        class TestClass
        {
            [Test]
            public static void Main()
            {
                var str = TestStruct<int>.X.ToString();
            }
        }
    }

    namespace Namespace1
    {
        class GClass1<T>
        {
        }
    }

    namespace Namespace2
    {
        class GClass1<T>
        {
            public static int Foo() { return 6; }
        }
    }

    public class GenericClasses
    {
        class GClass1<T>
        {
        }

        class GClass2<T>
        {
            T value;
        }

        class Class1
        {
        }

        class GClass3<T> : Class1
        {
        }

        class GClass4<T>
        {
            public void Foo(T o) { }
        }

        class GClass5<T> : GClass4<T>
        {
            public void Bar(T o) { }
        }

        class GClass6<T>
        {
            public GClass6(T o) { }
            public void Foo(T o) { }
        }

        class GClass7<T>
        {
            public static void Foo<U, V>(T t, U u, V v) { V w = v; }
        }

        class GClass8<T>
        {
            public static void Foo<U>(T t, U u) { }
        }

        class Class2
        {
            public void Foo<T>(T o) { }
        }

        class Class3
        {
            public Class3() { }
            public void Foo<T>(T o) { }
        }

        class GClass9<T>
        {
            T[] t;

            public GClass9(int n)
            {
                t = new T[n];
                for (int i = 0; i < n; i++) t[i] = default(T);
            }
        }

        class GClass10<T> : GClass9<T>
        {
            public GClass10(int n)
                : base(n)
            {
            }

            public void Foo<U>(U u)
            {
                if (Generic.Equals(u, default(U))) { }
            }
        }

        class Class4 : GClass6<int>
        {
            public Class4()
                : base(1)
            {
            }

            public void Bar(int i)
            {
                Foo(i);
            }
        }

        class Class5 : GClass6<Class5>
        {
            public Class5()
                : base(null) // Note: Using 'null' because 'this' is not allowed here
            {
            }

            public void Bar()
            {
                Foo(null);
            }
        }

        class GClass11<T>
        {
            public void Foo(T t)
            {
                bar(t);
            }

            void bar(object o)
            {
                var p = o;
            }
        }

        class GClass12<T>
        {
            public T BigT;
        }

        class Class6
        {
            public void Foo1() { }
        }

        class GClass13<T> : Class6
        {
            public void Foo2(T t) { }
        }

        class GClass14<T> : GClass13<T>
        {
            public void Foo3(T t) { }
        }

        class GClass15<T>
        {
            T t;

            public GClass15(T t)
            {
                this.t = t;
            }

            public object Foo()
            {
                return t;
            }
        }

        class GClass16<T>
        {
            T[] t;

            public GClass16(int n)
            {
                t = new T[n];
                for (int i = 0; i < n; i++) t[i] = default(T);
            }

            public object Foo()
            {
                return t;
            }
        }

        class GClass17<T>
        {
            public GClass17() { }
            public void Foo(T t) { }
        }

        class GClass18<T, U> : GClass17<U>
        {
            public GClass18() { }
            public void Bar(T t, U u) { }
        }

        class GClass19<T> : GClass1<GClass2<T>>
        {
        }

        class GClass20<T>
        {
            public void Foo(GClass20<T> o) { }
        }

        class GClass21<T>
        {
            protected class Node<U>
            {
                public readonly U Item;
                public readonly Node<U> Next;

                public Node(Node<U> next, U item)
                {
                    Next = next;
                    Item = item;
                }
            }

            protected Node<T> head;

            public GClass21(T first, T second)
            {
                head = new Node<T>(null, second);
                head = new Node<T>(head, first);
            }

            public T GetAt(int index)
            {
                var current = head;
                while (index-- > 0) current = current.Next;
                return current.Item;
            }
        }

        class GClass22<T>
        {
            public int Count;

            public GClass22(int count)
            {
                Count = count;
            }

            public static GClass22<T> operator +(GClass22<T> left, GClass22<T> right)
            {
                return new GClass22<T>(left.Count + right.Count);
            }
        }

        class GClass23<A, B>
        {
            public void Foo<U>(U u) { }
            public void Foo<V>(V[] v, V w) { }
            public void Foo<V, W>(V v, W w, GClass23<V, W> x) { }
        }

        delegate void GDelegate1<T>(T t);

        class GClass24<T>
        {
            event GDelegate1<T> method;
            public GClass24(GDelegate1<T> method) { this.method = method; }
            public void Invoke(T t) { if (method != null) method(t); }
        }

        class List // no type-args; using List should find this one, List<T> should find Uno.Collections.List<T>
        {
        }

        class GClass25<T>
        {
            void A(T t) { }
            void A(T[] t) { }
            void A(T[][] t) { }
        }

        class GClass26<T>
        {
            GClass26<T> x = null;
            public GClass26<T> Blah { get { return x; } }
        }

        class GClass27<T> : Namespace1.GClass1<T>
        {
        }

        delegate B GDelegate2<A, B>(A a);

        class GClass28<T>
        {
            T t;

            public GClass28(T t)
            {
                this.t = t;
            }

            public U Method<U>(GDelegate2<T, U> test)
            {
                var foo = this;
                return test(t);
            }
        }

        class Class7
        {
            public delegate int Foo<T>(T t, T u);
            public void Bar<U>(Foo<U> foo, U u) { }
        }

        class GClass29<T>
        {
            void create(int a) { }

            public GClass29(int a)
            {
                create(a);
            }

            public GClass29()
                //: this(Namespace2.GClass1<T>.Foo())
            {
                create(Namespace2.GClass1<T>.Foo());
            }
        }

        class GClass30<T>
        {
            public T Data1, Data2;

            public GClass30(T a, T b)
            {
                Data1 = a;
                Data2 = b;
            }
        }

        class GClass31<T>
        {
            public T Data1, Data2;
            public GClass31(T a, T b)
            {
                this.Data1 = a;
                this.Data2 = b;
            }
        }

        class GClass32<T>
        {
            public T Data;

            public GClass32(T data)
            {
                Data = data;
            }
        }

        class GClass33<T>
        {
            public GClass32<T> Foo(T data)
            {
                return new GClass32<T>(data);
            }
        }

        class GClass34<T>
        {
            protected class Foo
            {
                GClass34<T> bar;
                public Foo(GClass34<T> bar) { this.bar = bar; }
            }
        }

        class GClass35<T>
        {
            public virtual T[] ToArray() { return null; }
        }

        class GClass36<T> : GClass35<T> { }

        class GClass37<T> : GClass36<T> { }

        class GClass38<T> : GClass37<T>
        {
            public override T[] ToArray() { return null; }
        }

        struct GStruct1<T, U>
        {
            T field;
            public GStruct1(T t, U u) { field = t; }
            public GStruct1(T t) { field = t; }
        }

        class GClass39<T>
        {
            public void Foo(T t, out int a)
            {
                a = 5;
            }

            public void Bar(T t)
            {
                int a;
                Foo(t, out a);
            }
        }

        class GClass40<T>
        {
            Bar c;
            public Bar Foo<V>()
            {
                return c;
            }

            public class Bar { }
        }

        class GClass41<T>
        {
            public readonly T Item;

            public GClass41(T item)
            {
                Item = item;
            }

            public void GetItem(out T retval)
            {
                retval = Item;
            }

            public T GetItem(int a, ref T data)
            {
                return Item;
            }

            public void SetItem(T data) { }
        }

        class GClass42<T>
        {
            GClass41<Node> element;

            public Node Test()
            {
                Node n = element.Item;
                element.GetItem(out n);
                element.SetItem(n);
                return element.GetItem(3, ref n);
            }

            public class Node { }
        }

        class GClass43<T>
        {
            public readonly T Item;

            public GClass43(T item)
            {
                Item = item;
            }

            static void maketreer(out Node rest)
            {
                rest = new Node();
            }

            class Node { }

            public void Hello<U>()
            {
                GClass43<U>.Node n;
                GClass43<U>.maketreer(out n);
            }
        }

        class GClass44<T>
        {
            protected class Node
            {
            }
        }

        class GClass45<T> : GClass44<T>
        {
            Node n;
        }

        class GClass46<T>
        {
        }

        class Class8
        {
            static void Hello<T>(GClass46<T>[] foo, int i)
            {
                GClass46<T> element = foo[0];
                if (i > 0) Hello<T>(foo, i - 1);
            }

            public static void Foo<U>(GClass46<U>[] a)
            {
                Hello<U>(a, 1);
            }
        }

        class Class9
        {
            public Class9()
            {
                var s = new GStruct1<string>("lol");
            }
        }

        struct GStruct1<T>
        {
            T data;
            public GStruct1(T data)
            {
                this.data = data;
            }
        }

        class GClass47<T>
        {
            public void Foo() { }
        }

        class Class10 : GClass47<int>
        {
        }

        struct GStruct2<K, V>
        {
            public K Key;
            public V Value;
            public GStruct2(K k, V v) { Key = k; Value = v; }
            public GStruct2(K k) { Key = k; Value = default(V); }
        }

        class GClass48<T>
        {
            public readonly T Item;
            public GClass48(T item) { Item = item; }
            public void Find(ref T item) { item = Item; }
        }

        class Class11
        {
            public bool Foo()
            {
                var a = new GClass49<int>((GClass49<int>.Delegate)D, 3);
                var b = new GClass49<int>(D, 3);
                return a.Run() == -3;
            }

            public static int D(int x)
            {
                return -x;
            }
        }

        class GClass49<T>
        {
            public delegate T Delegate(T t);

            protected Delegate _d;
            protected T _value;

            public GClass49(Delegate d, T value)
            {
                _d = d;
                _value = value;
            }

            public T Run()
            {
                return _d(_value);
            }
        }

        class GClass50<T>
        {
            public delegate void Foo();
            public delegate void Bar<U>();
        }

        class GClass51<T>
        {
            public delegate void Changed(GClass51<T> a);

            protected event Changed _changed;

            public void Register(Changed changed)
            {
                _changed += changed;
                _changed(this);
            }
        }

        class Class12
        {
            public static bool Called = false;
            public void Run()
            {
                var a = new GClass51<int>();
                a.Register(Del);
            }
            public static void Del(GClass51<int> a)
            {
                Called = true;
            }
        }

        class GClass52<T>
        {
            T[] data;
            public GClass52(T[] data) { this.data = data; }
        }

        class GClass53<T>
        {
            protected Node first;

            protected class Node
            {
                public T item;

                public Node(T item)
                {
                    this.item = item;
                }
            }
        }

        class GClass54<U> : GClass53<U>
        {
            public void Insert(U x)
            {
                Node n = first;
            }
        }

        class GClass55<A>
        {
            public static void Foo<B>(GClass55<B> x) { Class13.Append(x); }
        }

        class Class13
        {
            public static void Append<A>(GClass55<A> x) { }
            public static void Foo()
            {
                GClass55<object>.Foo<int>(null);
            }
        }

        class GClass56<A> { }

        abstract class GClass57<A, B, R>
        {
            public abstract R Apply(A x, B y);
        }

        class GClass58<A> : GClass57<GClass56<A>, GClass56<A>, GClass56<A>>
        {
            public override GClass56<A> Apply(GClass56<A> x, GClass56<A> y)
            {
                return Class14.RevAppend(x, y);
            }
        }

        class Class14
        {
            public static B FoldLeft<A, B>(A x, B acc, GClass57<A, B, B> f)
            {
                return f.Apply(x, acc);
            }

            public static GClass56<A> RevAppend<A>(GClass56<A> x, GClass56<A> y)
            {
                return x;
            }
        }

        class GClass59<T>
        {
            public abstract class Node { }
            public class ConcatNode : Node { }
            public Node GetRoot() { return new ConcatNode(); }
        }

        class GClass60<T, U>
        {
        }

        class GClass61<T, U>
        {
            private GClass62<GClass60<T, U>> tree;
            private void EnumKeys(GClass62<GClass60<T, U>>.RangeTester rangeTester)
            {
                tree.EnumerateRange(rangeTester);
            }
        }

        class GClass62<S>
        {
            public delegate int RangeTester(S item);
            public void EnumerateRange(RangeTester rangeTester) { }
        }
/*
        class GClass63<T> : List<T>
        {
        }

        class GClass64<T>
        {
            private GClass63<T> list = new GClass63<T>();
            public void AddItem(T item) { list.Add(item); }
        }
*/
        class GClass65<T>
        {
            public struct Node
            {
                private int foo;
                private T bar;
            }
        }

        class GClass66<T>
        {
            public static T ToT(object o)
            {
                return (T)o;
            }

            public static object ToObject(T t)
            {
                return (object)t;
            }
        }

        GClass4<int> gObj1;
        GClass5<float> gObj2;
        GClass7<float2> gObj3;
        GClass8<int4> gObj4;
        GClass9<float3> gObj5 = new GClass9<float3>(12);
        GClass14<int> gObj6 = new GClass14<int>();
        GClass19<string> gObj7 = new GClass19<string>();

        void foo1(GClass1<Class1> a) { }
        void foo2() { gObj1.Foo(3); }
        void foo3() { gObj2.Foo(5); gObj2.Bar(3.4f); }
        void foo4(Class3 class3) { class3.Foo<double>(5.6); }
        void foo5(GClass18<string, int> a) { a.Foo(6); a.Bar("hehe", 9); }
        void foo6(int i) { }
        bool foo7(int a) { return a == 34; }
        string foo8(int a) { return a.ToString(); }
        static int foo9(int a, int b) { return a + b; }

        abstract class Abstract
        {
            public abstract T Foo<T>(T t);
        }

        class Concrete : Abstract
        {
            public override T Foo<T>(T t)
            {
                return t;
            }
        }

        class Derived : Concrete
        {
            new public T Foo<T>(T t)
            {
                return default(T);
            }
        }

        public class ThingHolder<T>
        {
            static int TheThing;
            public ThingHolder(T x) { TheThing = 1; }
        }

        int CallVirtualOnNull1<T>(T key)
        {
            return key.GetHashCode();
        }

        void CallVirtualOnNull2()
        {
            CallVirtualOnNull1((string)null);
        }

        [Test]
        public void Run()
        {
            Assert.Throws<NullReferenceException>(CallVirtualOnNull2);
            new ThingHolder<float>(1.2f);

            var d = new Derived();
            Abstract a = d;
            Assert.AreEqual(1, a.Foo(1));
            Assert.AreEqual(0, d.Foo(1));

            var i1 = new GClass1<Class1>();
            foo1(i1);
            gObj1 = new GClass4<int> ();
            gObj2 = new GClass5<float> ();
            gObj3 = new GClass7<float2>();
            gObj4 = new GClass8<int4>();
            foo2();
            foo3();

            var i2 = new GClass6<int>(-12);
            i2.Foo(8);
            var i3 = new GClass6<string>("lawl");
            i3.Foo("herpderp");

            var i4 = new Class2();
            i4.Foo<string>("stringparam");
            i4.Foo<sbyte2>(sbyte2(12, -1));

            var i5 = new Class3();
            foo4(i5);

            var i6 = new GClass10<float>(2);
            i6.Foo<string>("haha");

            var i7 = new Class4();
            i7.Bar(4);

            var i9 = new GClass11<double>();
            i9.Foo(23.45);

            var i10 = new GClass12<int>();
            i10.BigT = 6;
            var i11 = i10.BigT;
            Assert.AreEqual(6, i11);

            gObj6.Foo1();
            gObj6.Foo2(5);
            gObj6.Foo3(-12);

            var i12 = new GClass15<float>(4.5f);
            Assert.AreEqual(4.5f, (float)i12.Foo());

            var i13 = new GClass16<int>(1);
            Assert.AreEqual(default(int), ((int[])i13.Foo())[0]);

            var i14 = new GClass18<int, float>();
            i14.Foo(4.56f);
            i14.Bar(3, -12.5f);

            foo5(new GClass18<string, int>());

            var i15 = new GClass20<int>();
            i15.Foo(i15);

            var i16 = new GClass21<int>(1, 2);
            Assert.AreEqual(1, i16.GetAt(0));
            Assert.AreEqual(2, i16.GetAt(1));

            var i17 = new GClass22<long>(4);
            Assert.AreEqual(4, i17.Count);

            var i18 = i17 + new GClass22<long>(7);
            Assert.AreEqual(11, i18.Count);

            var i19 = new GClass23<float, int>();
            i19.Foo("string");
            i19.Foo(new int[] { 3, 4, 5 }, 9);
            i19.Foo(4.5f, 6, i19);

            var i20 = new GClass24<int>(foo6);
            i20.Invoke(8);

            var i21 = new List<int>();

            var i22 = new Dictionary<int, Class1>();
            i22.Add(5, new Class1());

            var i23 = new GClass25<float>();

            var i24 = new GClass26<int>();
            Assert.IsTrue(i24.Blah == null);

            var i25 = new GClass27<ushort>();

            var i26 = new GClass28<int>(34);
            Assert.IsTrue(i26.Method<bool>(foo7));
            Assert.AreEqual("34", i26.Method<string>(foo8));

            var i27 = new Class7();
            i27.Bar((Class7.Foo<int>)foo9, 6);

            var i28 = new GClass29<double>();

            var i29 = new GClass30<long>(3, 5);
            Assert.AreEqual(3, i29.Data1);
            Assert.AreEqual(5, i29.Data2);
            var i30 = new GClass31<long>(3, 5);
            Assert.AreEqual(3, i30.Data1);
            Assert.AreEqual(5, i30.Data2);

            var i31 = new GClass33<long>();
            Assert.AreEqual(0x800, i31.Foo(0x800).Data);

            GClass35<int> i32 = new GClass38<int>();
            i32.ToArray();

            var i33 = new GClass39<float>();
            i33.Bar(3.4f);

            var i44 = new GClass43<int>(6);
            i44.Hello<float>();

            GClass46<int>[] i45 = new GClass46<int>[1];
            i45[0] = new GClass46<int>();
            Class8.Foo(i45);

            var i46 = new Class9();

            object i47 = new Class10();
            var i48 = (GClass47<int>)i47;
            i48.Foo();

            var i49 = new GStruct2<int, long>(3);
            var i50 = new GStruct2<int, long>(5, 9);
            var i51 = new GClass48<GStruct2<int, long>>(i49);
            Assert.AreEqual(5, i50.Key);
            Assert.AreEqual(9, i50.Value);
            Assert.AreEqual(3, i49.Key);
            Assert.AreEqual(default(long), i49.Value);
            i51.Find(ref i49);

            var item = i51.Item;

            Assert.AreEqual(3, item.Key);
            Assert.AreEqual(default(long), item.Value);

            var i52 = new Class11();
            Assert.IsTrue(i52.Foo());

            Assert.IsFalse(Class12.Called);
            var i53 = new Class12();
            i53.Run();
            Assert.IsTrue(Class12.Called);

            var i54 = new GClass52<double>(new double[5]);

            Class13.Foo();

            var i55 = new GClass59<int>();
            GClass59<int>.Node root = i55.GetRoot();

            // Casting T <=> object
            var i56 = 10.0;
            var i57 = GClass66<double>.ToObject(i56);
            var i58 = GClass66<double>.ToT(i57);
            Assert.IsTrue(i56 == i58);
            Assert.IsTrue(i58 == (double)i57);
        }
    }
}
