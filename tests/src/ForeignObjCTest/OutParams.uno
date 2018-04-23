using Uno;
using Uno.Compiler.ExportTargetInterop;
using Uno.Testing;

namespace ForeignObjCTest
{
    public extern(FOREIGN_OBJC_SUPPORTED) class OutParams
    {
        [Foreign(Language.ObjC)]
        void primitiveOutParam(ref int m, out int n)
        @{
            *m = 222;
            *n = 123;
            /* I'm a comment */
            // Another comment
        @}

        [Test]
        public void PrimitiveOutParam()
        {
            int m = 0; int n = 0;
            primitiveOutParam(ref m, out n);
            Assert.AreEqual(222, m);
            Assert.AreEqual(123, n);
        }

        [Foreign(Language.ObjC)]
        void objectOutParam(ref ObjC.Object m, out ObjC.Object n)
        @{
            *m = @"Out1";
            /* I'm a comment */
            // Another comment
            *n = @"Out2";
        @}

        [Foreign(Language.ObjC)]
        string asString(ObjC.Object o) @{ return o; @}

        [Test]
        public void ObjectOutParam()
        {
            ObjC.Object x = null;
            ObjC.Object y = null;
            objectOutParam(ref x, out y);
            Assert.AreEqual("Out1", asString(x));
            Assert.AreEqual("Out2", asString(y));
        }

        [Foreign(Language.ObjC)]
        void stringOutParam(ref string m, out string n)
        @{
            /* I'm a comment */
            // Another comment
            *m = [*m stringByAppendingString:@"Out1"];
            *n = @"Out2";
        @}

        [Test]
        public void StringOutParam()
        {
            var x = "In1";
            var y = "In2";
            stringOutParam(ref x, out y);
            Assert.AreEqual(x, "In1Out1");
            Assert.AreEqual(y, "Out2");
        }

        [Foreign(Language.ObjC)]
        void funcOutParam(ref Func<string, string> f, out Func<string, string> g)
        @{
            *f = ^ NSString* (NSString* x)
            {
            /* I'm a comment */
            // Another comment
                return [x stringByAppendingString:@"Out"];
            };
            *g = ^ NSString* (NSString* x)
            {
            /* I'm a comment */
            // Another comment
                return [x stringByAppendingString:@"Out"];
            };
        @}

        [Test]
        public void FuncOutParam()
        {
            Func<string,string> f = null;
            Func<string,string> g;
            funcOutParam(ref f, out g);
            Assert.AreEqual("InOut", f("In"));
            Assert.AreEqual("InOut", g("In"));
        }

        [Foreign(Language.ObjC)]
        void arrayOutParam(ref string[] arr1, out string[] arr2)
        @{
            *arr1 = @{string[]:New(3)};
            (*arr1)[0] = @"elem1";
            (*arr1)[1] = @"elem2";
            (*arr1)[2] = @"elem3";
            /* I'm a comment */
            // Another comment

            *arr2 = @{string[]:New(3)};
            (*arr2)[0] = @"elem1";
            (*arr2)[1] = @"elem2";
            (*arr2)[2] = @"elem3";
            /* I'm a comment */
            // Another comment
        @}

        [Test]
        public void ArrayOutParam()
        {
            string[] arr1 = null;
            string[] arr2 = null;
            arrayOutParam(ref arr1, out arr2);
            Assert.AreEqual(arr1[0], arr2[0]);
            Assert.AreEqual(arr1[1], arr2[1]);
            Assert.AreEqual(arr1[2], arr2[2]);
        }

        public class MyClass
        {
            public string Field;
            public MyClass(string s) { Field = s; }
        }

        [Foreign(Language.ObjC)]
        void classOutParam(ref MyClass obj1, out MyClass obj2)
        @{
            /* I'm a comment */
            // Another comment
            *obj1 = @{MyClass(string):New(@"Out1")};
            /* I'm a comment */
            // Another comment
            *obj2 = @{MyClass(string):New(@"Out2")};
            /* I'm a comment */
            // Another comment
        @}


        [Test]
        public void ClassOut()
        {
            MyClass obj1 = null;
            MyClass obj2 = null;
            classOutParam(ref obj1, out obj2);
            Assert.AreEqual("Out1", obj1.Field);
            Assert.AreEqual("Out2", obj2.Field);
        }

        delegate void PrimitiveOut(out int n);

        [Foreign(Language.ObjC)]
        void outDelegatePrimitiveOut(out PrimitiveOut f, ref PrimitiveOut g)
        @{
            /* I'm a comment */
            // Another comment
            *f = ^ void (int* n)
            {
            /* I'm a comment */
            // Another comment
                *n = 123;
            };

            *g = ^ void (int* n)
            {
            /* I'm a comment */
            // Another comment
                *n = 567;
            };
            /* I'm a comment */
            // Another comment
        @}

        [Test]
        public void OutDelegatePrimitiveOut()
        {
            PrimitiveOut f = null;
            PrimitiveOut g = null;
            outDelegatePrimitiveOut(out f, ref g);
            int m, n;
            f(out m);
            g(out n);

            Assert.AreEqual(123, m);
            Assert.AreEqual(567, n);
        }

        delegate void ObjectOut(ref ObjC.Object o);

        [Foreign(Language.ObjC)]
        void outDelegateObjectOut(out ObjectOut f, ref ObjectOut g)
        @{
            /* I'm a comment */
            // Another comment
            *f = ^ void (id* n)
            {
                *n = @"abc";
            };

            *g = ^ void (id* n)
            {
                *n = @"123";
            };
            /* I'm a comment */
            // Another comment
        @}

        [Test]
        public void OutDelegateObjectOut()
        {
            ObjectOut f = null;
            ObjectOut g = null;
            outDelegateObjectOut(out f, ref g);
            ObjC.Object m = null, n = null;
            f(ref m);
            g(ref n);
            Assert.AreEqual("abc", asString(m));
            Assert.AreEqual("123", asString(n));
        }

        delegate void StringOut(out string s);

        [Foreign(Language.ObjC)]
        void outDelegateStringOut(out StringOut f, ref StringOut g)
        @{
            *f = ^ void (NSString** s)
            {
                *s = @"abc";
            };

            *g = ^ void (NSString** s)
            {
                *s = @"123";
            };
        @}

        [Test]
        public void OutDelegateStringOut()
        {
            StringOut f = null;
            StringOut g = null;
            outDelegateStringOut(out f, ref g);
            string m = null, n = null;
            f(out m);
            g(out n);
            Assert.AreEqual("abc", m);
            Assert.AreEqual("123", n);
        }

        delegate void FuncOut(ref Func<string, string> s);

        [Foreign(Language.ObjC)]
        void outDelegateFuncOut(out FuncOut f, ref FuncOut g)
        @{
            /* I'm a comment */
            // Another comment
            *f = ^ void (uObjC::Function<NSString*, NSString*>* s)
            {
                *s = ^ NSString* (NSString* s) {
            /* I'm a comment */
            // Another comment
                    return [s stringByAppendingString:@"Out"];
                };
            };
            *g = ^ void (uObjC::Function<NSString*, NSString*>* s)
            {
            /* I'm a comment */
            // Another comment
                *s = ^ NSString* (NSString* s) {
                    return [s stringByAppendingString:@"Out"];
                };
            };
        @}

        [Test]
        public void OutDelegateFuncOut()
        {
            FuncOut f = null;
            FuncOut g = null;
            outDelegateFuncOut(out f, ref g);
            Func<string, string> m = null, n = null;
            f(ref m);
            g(ref n);
            Assert.AreEqual("InOut", m("In"));
            Assert.AreEqual("InOut", n("In"));
        }

        delegate void ArrayOut(out string[] s);

        [Foreign(Language.ObjC)]
        void outDelegateArrayOut(out ArrayOut f, ref ArrayOut g)
        @{
            *f = ^ void (id<UnoArray>* a)
            {
                *a = @{string[]:New(2)};
                (*a)[0] = @"abc";
                (*a)[1] = @"123";
            };
            *g = ^ void (id<UnoArray>* a)
            {
                *a = @{string[]:New(2)};
                (*a)[0] = @"abc";
                (*a)[1] = @"123";
            };
            /* I'm a comment */
            // Another comment
        @}

        [Test]
        public void OutDelegateArrayOut()
        {
            ArrayOut f = null;
            ArrayOut g = null;
            outDelegateArrayOut(out f, ref g);
            string[] m = null, n = null;
            f(out m);
            g(out n);
            Assert.AreEqual("abc", m[0]);
            Assert.AreEqual("abc", n[0]);
            Assert.AreEqual("123", m[1]);
            Assert.AreEqual("123", n[1]);
        }

        delegate void ClassOutDelegate(ref MyClass c);

        [Foreign(Language.ObjC)]
        void outDelegateClassOut(out ClassOutDelegate f, ref ClassOutDelegate g)
        @{
            /* I'm a comment */
            // Another comment
            *f = ^ void (id<UnoObject>* a)
            {
                *a = @{MyClass(string):New(@"abc")};
            };
            *g = ^ void (id<UnoObject>* a)
            {
                *a = @{MyClass(string):New(@"123")};
            };
            /* I'm a comment */
            // Another comment
        @}

        [Test]
        public void OutDelegateClassOut()
        {
            ClassOutDelegate f = null;
            ClassOutDelegate g = null;
            outDelegateClassOut(out f, ref g);
            MyClass m = null, n = null;
            f(ref m);
            g(ref n);
            Assert.AreEqual("abc", m.Field);
            Assert.AreEqual("123", n.Field);
        }
    }
}
