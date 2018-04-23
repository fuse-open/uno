using Uno;
using Uno.Compiler.ExportTargetInterop;
using Uno.Testing;

namespace ForeignObjCTest
{
    public extern(FOREIGN_OBJC_SUPPORTED) class Nulls
    {
        [ForeignTypeName("::NSURL*")]
        public class NSURL : ObjC.Object { }

        [Foreign(Language.ObjC)]
        static string NullStrings(string x)
        @{
            return nil;
        @}

        [Foreign(Language.ObjC)]
        static ObjC.Object NullObjects(ObjC.Object x)
        @{
            return nil;
        @}

        [Foreign(Language.ObjC)]
        static ObjC.Object NullTypeNameObjects(NSURL x)
        @{
            return nil;
        @}

        [Foreign(Language.ObjC)]
        static object NullUnoObjects(object x)
        @{
            return nil;
        @}

        [Foreign(Language.ObjC)]
        static Func<int, string> NullDelegates(Func<int, string> x)
        @{
            return nil;
        @}

        [Foreign(Language.ObjC)]
        static string[] NullArrays(string[] x)
        @{
            return nil;
        @}

        [Foreign(Language.ObjC)]
        static string NullStrings2(string x)
        @{
            return x;
        @}

        [Foreign(Language.ObjC)]
        static ObjC.Object NullObjects2(ObjC.Object x)
        @{
            return x;
        @}

        [Foreign(Language.ObjC)]
        static object NullUnoObjects2(object x)
        @{
            return x;
        @}

        [Foreign(Language.ObjC)]
        static Func<int, string> NullDelegates2(Func<int, string> x)
        @{
            return x;
        @}

        [Foreign(Language.ObjC)]
        static string[] NullArrays2(string[] x)
        @{
            return x;
        @}

        [Foreign(Language.ObjC)]
        static ObjC.Object NullTypeNameObjects2(NSURL x)
        @{
            return x;
        @}

        [Test]
        public void Test()
        {
            Assert.AreEqual(null, NullStrings(null));
            Assert.AreEqual(null, NullObjects(null));
            Assert.AreEqual(null, NullTypeNameObjects(null));
            Assert.AreEqual(null, NullUnoObjects(null));
            Assert.AreEqual(null, NullDelegates(null));
            Assert.IsTrue(null == NullArrays(null));
            Assert.AreEqual(null, NullStrings2(null));
            Assert.AreEqual(null, NullObjects2(null));
            Assert.AreEqual(null, NullTypeNameObjects2(null));
            Assert.AreEqual(null, NullUnoObjects2(null));
            Assert.AreEqual(null, NullDelegates2(null));
            Assert.IsTrue(null == NullArrays2(null));
        }
    }
}
