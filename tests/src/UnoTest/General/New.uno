using Uno;
using Uno.Collections;
using Uno.Testing;

namespace UnoTest.General
{
    public class New
    {
        static float Variable = 1;

        class Class
        {
            public static void Method()
            {
                int a = (int)Variable;
                Variable += a;
            }
        }

        class Lol
        {
            public static void foo()
            {
            }
        }

        static void foo()
        {
        }

        static int bar { get { return 0; } set { } }
        static int baz = 0;

        class Fields
        {
            public int A;
            public float B;
        }

        protected internal static int Prop1
        {
            get;
            protected set;
        }

        static int _Prop2;
        internal static float Prop2
        {
            private get { return 5; }
            set { _Prop2 = (int)value; }
        }

        private static int _IndexerValue;
        public int this[int index]
        {
            get { return -1; }
            protected internal set { _IndexerValue = value; }
        }

        public class Contact
        {
            string name;
            List<string> phoneNumbers = new List<string>();

            public string Name
            {
                get { return name; }
                set { name = value; }
            }

            public List<string> PhoneNumbers
            {
                get { return phoneNumbers; }
            }
        }


        static int Field1 = 32;

        [Test]
        public void Run()
        {
            bar = 2;
            baz = 1;

            foo();
            Lol.foo();

            Class.Method();
            Assert.AreEqual(2, Variable);

            // Array initializers

            var ints = new[] { 1, 2, 3, 4, 5, };
            Assert.IsTrue(ints is int[]);
            Assert.IsTrue(ints is object);

            var floats = new[] { 1, 1.5f, 2 };
            Assert.IsTrue(floats is float[]);

            var doubles = new[] { 1, 1.5f, 2.0 };
            Assert.IsTrue(doubles is double[]);

            var bytes = new byte[] { 1, 2, 3, 4, 5 };
            Assert.IsTrue(bytes is byte[]);
            Assert.AreEqual(3, bytes[2]);

            var bytes2 = new[] { (byte)1, 2, 3, 4, 5 };
            Assert.IsTrue(bytes2 is byte[]);

            var ints2 = new[] { 1, (byte)2, 3, 4, 5 };
            Assert.IsTrue(ints2 is int[]);


            // Jagged arrays

            var intInts1 = new int[2][]
            {
                new int[3] { 1, 2, 3 },
                new int[2] { 4, 5 }
            };

            var intInts2 = new int[][]
            {
                new[] { 1, 2, 3 },
                new[] { 4, 5 }
            };

            var intInts3 = new[]
            {
                new[] { 1, 2, 3 },
                new[] { 4, 5 }
            };

            var intInts4 = new int[4][];

            var intIntInts1 = new int[14][][];
            intIntInts1[0] = intInts1;
            intIntInts1[1] = intInts2;
            intIntInts1[2] = intInts3;
            intIntInts1[3] = intInts4;

            var intIntInts2 = new int[][][]
            {
                null,
                intInts1,
                intInts2,
                intInts3,
                intInts4,
            };

            var intIntInts3 = new[]
            {
                null,
                intInts1,
                intInts2,
                intInts3,
                intInts4,
            };

            Assert.IsTrue(intIntInts3[1] == intInts1);
            Assert.IsTrue(intIntInts1[5] == intIntInts2[0]);
            Assert.IsTrue(intIntInts2[0] == intIntInts3[0]);
            Assert.IsTrue(intIntInts3[2][1][0] == 4);
            Assert.IsTrue(intIntInts2[2][1][0] == 4);


            // Collection initializers

            var map = new Dictionary<int, int>()
            {
                { 1, 2 },
                { 2, 3 },
                { 3, 4 },
                { 4, 5 },
            };

            Assert.AreEqual(4, map.Count);
            Assert.AreEqual(2, map[1]);
            Assert.AreEqual(3, map[2]);

            var list = new List<ushort>() { 2, 3 };

            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(2, list[0]);
            Assert.AreEqual(3, list[1]);

            var arrayList = new List<int[]>() { null, new [] { 1 }, new int[2], };
            var arrayList2 = arrayList as object as List<int[]> as object as List<int[]>;

            Assert.IsTrue(arrayList2 != null);
            Assert.IsTrue(arrayList2[0] == null);
            Assert.AreEqual(1, arrayList2[1][0]);
            Assert.AreEqual(0, arrayList2[2][0]);


            // Member initializers

            var fields = new Fields()
            {
                A = 1,
                B = 2,
            };

            Assert.AreEqual(1, fields.A);
            Assert.AreEqual(2, fields.B);

            var contacts1 = new List<Contact>
            {
                new Contact
                {
                    Name = "Onkel",
                    PhoneNumbers = { "206-555-0101", "425-882-8080" }
                },
                new Contact
                {
                    Name = "Tante",
                    PhoneNumbers = { "650-555-0199" }
                }
            };

            Assert.AreEqual("425-882-8080", contacts1[0].PhoneNumbers[1]);


            // Chained assigns

            var arr = new int[1];

            int v1 = this[0] = (int)(Prop2 = arr[0] = Field1 = Prop1 = 4);
            Assert.AreEqual(4, v1);
            Assert.AreEqual(4, _IndexerValue);
            Assert.AreEqual(4, _Prop2);
            Assert.AreEqual(4, Prop1);
            Assert.AreEqual(5, Prop2);
            Assert.AreEqual(4, Field1);
            Assert.AreEqual(4, arr[0]);

            int v2 = Field1 = this[0] = arr[0] = Prop1 = v1 = (int)(Prop2 = 7);
            Assert.AreEqual(7, v1);
            Assert.AreEqual(7, v2);
            Assert.AreEqual(7, _IndexerValue);
            Assert.AreEqual(7, _Prop2);
            Assert.AreEqual(7, Prop1);
            Assert.AreEqual(5, Prop2);
            Assert.AreEqual(7, Field1);
            Assert.AreEqual(7, arr[0]);

            int v3 = fields.A = this[0] = this[0];
            Assert.AreEqual(-1, v3);
            Assert.AreEqual(-1, fields.A);
            Assert.AreEqual(-1, _IndexerValue);
        }
    }
}
