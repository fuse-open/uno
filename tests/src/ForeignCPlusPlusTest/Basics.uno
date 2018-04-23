using Uno;
using Uno.Compiler.ExportTargetInterop;
using Uno.Testing;

namespace ForeignCPlusPlusTest
{
    [Require("Source.Include", "cheader.h")]
    public extern(CPlusPlus || PInvoke || ENABLE_CIL_TESTS) class Basics
    {
        [Foreign(Language.CPlusPlus)]
        static int ReturnInt()
        @{
            return 123;
        @}

        [Foreign(Language.CPlusPlus)]
        static int UseInt(int a, int b)
        @{
            return a + b;
        @}

        [Foreign(Language.CPlusPlus)]
        static int UseString(string s)
        @{
            return s[0] + s[1];
        @}

        [Foreign(Language.CPlusPlus)]
        static int UseArray(byte[] arr)
        @{
            return arr[0] + arr[1];
        @}

        [Foreign(Language.CPlusPlus)]
        static void ModifyArray(int[] arr)
        @{
            arr[0] = 12;
            arr[1] = 14;
        @}

        [Foreign(Language.CPlusPlus)]
        static void RefParam(byte[] arr, ref int result)
        @{
            *result = arr[0] + arr[1];
        @}

        [Foreign(Language.CPlusPlus)]
        static bool UseBool(bool b)
        @{
            return b;
        @}

        [Foreign(Language.CPlusPlus)]
        static bool ReturnTrue()
        @{
            return true;
        @}

        [Foreign(Language.CPlusPlus)]
        static bool ReturnFalse()
        @{
            return false;
        @}

        [Foreign(Language.CPlusPlus)]
        static int ReturnIntFromBool(bool b)
        @{
            return b ? 1 : 0;
        @}

        [Foreign(Language.CPlusPlus)]
        static int UseExternal()
        @{
            return ::external();
        @}

        [Test]
        public void Tests()
        {
            Assert.AreEqual(123, ReturnInt());
            Assert.AreEqual(3, UseInt(1, 2));
            Assert.AreEqual((int)'a' + (int)'b', UseString("abc"));
            Assert.AreEqual(1 + 2, UseArray(new byte[] { 1, 2 }));
            {
                int result = 0;
                RefParam(new byte[] { 1, 2 }, ref result);
                Assert.AreEqual(1 + 2, result);
            }
            {
                var arr = new int[2];
                ModifyArray(arr);
                Assert.AreEqual(12, arr[0]);
                Assert.AreEqual(14, arr[1]);
            }

            Assert.AreEqual(34, UseExternal());
            Assert.AreEqual(true, UseBool(true));
            Assert.AreEqual(false, UseBool(false));
            Assert.AreEqual(true, ReturnTrue());
            Assert.AreEqual(false, ReturnFalse());
            Assert.AreEqual(1, ReturnIntFromBool(true));
            Assert.AreEqual(0, ReturnIntFromBool(false));
        }
    }

    public extern(!CPlusPlus) class Dummy
    {
        [Test]
        public void Test()
        {
            Assert.AreEqual(0, 0);
        }
    }
}
