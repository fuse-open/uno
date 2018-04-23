using Uno;
using Uno.Compiler.ExportTargetInterop;
using Uno.Testing;

namespace ForeignCPlusPlusTest
{
    public extern(CPlusPlus || PInvoke || ENABLE_CIL_TESTS) class TypeMacros
    {
        [Foreign(Language.CPlusPlus)]
        static int ReturnInt()
        @{
            @{int} myInt = 123;
            @{char} myChar = 123;
            @{short} myShort = 123;
            @{ushort} myUShort = 123;
            @{uint} myUInt = 123;
            @{long} myLong = 123;
            @{ulong} myULong = 123;
            @{byte} myByte = 123;
            @{sbyte} mySByte = 123;

            @{string} myString = &myChar;
            @{int[]} myIntArray = &myInt;
            @{char[]} myCharArray = &myChar;
            @{short[]} myShortArray = &myShort;
            @{ushort[]} myUShortArray = &myUShort;
            @{uint[]} myUIntArray = &myUInt;
            @{long[]} myLongArray = &myLong;
            @{ulong[]} myULongArray = &myULong;
            @{byte[]} myByteArray = &myByte;
            @{sbyte[]} mySByteArray = &mySByte;
            return myInt;
        @}

        [Test]
        public void Tests()
        {
            Assert.AreEqual(123, ReturnInt());
        }
    }
}
