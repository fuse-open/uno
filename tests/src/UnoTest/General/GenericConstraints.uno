using Uno;
using Uno.Collections;
using Uno.Testing;

namespace UnoTest.General
{
    public class GenericConstraints
    {
        public class Employee
        {
            private string name;
            private int id;

            public Employee()
            {

            }

            public Employee(string s, int i)
            {
                name = s;
                id = i;
            }

            public string Name
            {
                get { return name; }
                set { name = value; }
            }

            public int ID
            {
                get { return id; }
                set { id = value; }
            }
        }

        public class BaseClass {}

        public class GenericDerived<T> : BaseClass where T : Employee, new()
        {
            public T Create()
            {
                return new T();
            }
        }

        public class GenericList<T> where T : Employee
        {
            private class Node
            {
                private Node next;
                private T data;

                public Node(T t)
                {
                    next = null;
                    data = t;
                }

                public Node Next
                {
                    get { return next; }
                    set { next = value; }
                }

                public T Data
                {
                    get { return data; }
                    set { data = value; }
                }
            }

            private Node head;

            public GenericList() //constructor
            {
                head = null;
            }

            public void AddHead(T t)
            {
                Node n = new Node(t);
                n.Next = head;
                head = n;
            }

            public IEnumerator<T> GetEnumerator()
            {
                /*
                Node current = head;

                while (current != null)
                {
                    yield return current.Data;
                    current = current.Next;
                }
                */

                return null;
            }

            public T FindFirstOccurrence(string s)
            {
                Node current = head;
                T t = null;

                while (current != null)
                {
                    //The constraint enables access to the Name property.
                    if (current.Data.Name == s)
                    {
                        t = current.Data;
                        break;
                    }
                    else
                    {
                        current = current.Next;
                    }
                }
                return t;
            }
        }

        class MyClass<T, U>
            where T : class
            where U : struct
        {
        }

        public void OpTest<T>(T s, T t) where T : class
        {
            Assert.IsTrue(s == t);
        }

        class Foo
        {
            public int Field;

            public Foo()
            {
                Field = 1;
            }
        }

        T Create<T>() where T : new()
        {
            return new T();
        }

        class Wrapper<T> where T : Foo
        {
            public T Field { get; private set; }

            public Wrapper(T t)
            {
                Field = t;
            }

            public void Foo()
            {
                Field.Field = 2;
            }
        }

        public class Foo2
        {
            T Bar<T>() where T : Foo2
            {
                return this as T ?? null;
            }
        }

        [Test]
        public void Run()
        {
            var obj = Create<Foo>();
            Assert.IsTrue(obj.Field == 1);
            new Wrapper<Foo>(obj).Foo();
            Assert.IsTrue(obj.Field == 2);

            var list = new GenericList<Employee>();
            OpTest("foo", "foo");
            test(1, 2);
        }

        // https://github.com/fusetools/Uno/issues/427
        struct TesterData
        {
          public long Data1;
          public long Data2;
          public long Data3;
        }

        List<TesterData> mList = new List<TesterData>();

        public GenericConstraints()
        {
          var d1 = new TesterData();
          d1.Data1 = 4848;
          d1.Data2 = 6682;
          d1.Data3 = 4983;
          mList.Add(d1);
        }

        public void test(long p1, long p2)
        {
          long pp3 = 3;

          if(p1 != 1 || p2 != 2 || pp3 != 3)
          throw new Exception();

          /* this call accesses memory of p2 */
          var enumer = mList.GetEnumerator();

          /* p2 is 0 */
          if(p1 != 1 || p2 != 2 || pp3 != 3)
          throw new Exception();

          enumer.MoveNext();

          if(p1 != 1 || p2 != 2 || pp3 != 3)
          throw new Exception();
        }

        interface IComparable<T>
        {
            int CompareTo(T o);
        }

        class InterfaceConstraint
        {
            public static int Foo<T>(this IEnumerable<T> values) where T : IComparable<T>
            {
                foreach (T value1 in values)
                    foreach(T value2 in values)
                        return value1.CompareTo(value2);
                return 0;
            }
        }
    }
}

namespace ConstraintBugs
{
    public partial class MainView
    {
        public MainView()
        {
            Level1<Object, Level2<Object>> one = new Level1<Object, Level2<Object>>();
        }

        public T FindByType<T>() where T : object
        {
            var tmp = this as T;
            if (tmp != null)
                return tmp;
            return null;
        }
    }
    ​
    public class Base<T>
        where T : class
    {
        public Base()
        {
        }
    }
    ​
    public class Level1<T1, T2> : Base<T1>
        where T1 : class
        where T2 : Level2<T1>
    {
        public Level1()
        {
        }
    }
    ​
    public class Level2<TL2>
        where TL2 : class
    {
        public Level2()
        {
        }
    }

    public partial class MainView1
    {
        public MainView1()
        {
            IBox<byte> box = new Cupboard.ByteBox();
            box.Content = 255;
        }
    }

    public interface IBox<T>
    {
        T Content { get; set; }
    }

    public class Cupboard
    {
        public struct ByteBox : IBox<byte>
        {
            public byte Content { get; set; }
        }
    }
}
