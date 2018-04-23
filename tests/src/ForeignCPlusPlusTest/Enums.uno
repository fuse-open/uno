using Uno;
using Uno.Compiler.ExportTargetInterop;
using Uno.Testing;

namespace ForeignCPlusPlusTest
{
    enum MyEnum : int
    {
        Value1,
        Value2,
    }

    enum MyByteEnum : byte
    {
        Value1 = (byte)0,
        Value2 = (byte)1,
    }

    public extern(CPlusPlus || PInvoke || ENABLE_CIL_TESTS) class EnumTests
    {
        [Foreign(Language.CPlusPlus)]
        static MyEnum UseEnum(MyEnum e)
        @{
            return (@{MyEnum})e;
        @}

        [Foreign(Language.CPlusPlus)]
        static MyByteEnum UseByteEnum(MyByteEnum e)
        @{
            return e;
        @}

        [Foreign(Language.CPlusPlus)]
        static MyEnum ReturnEnumValue1() @{ return @{MyEnum.Value1}; @}
        [Foreign(Language.CPlusPlus)]
        static MyEnum ReturnEnumValue2() @{ return @{MyEnum.Value2}; @}
        [Foreign(Language.CPlusPlus)]
        static MyByteEnum ReturnByteEnumValue1() @{ return @{MyByteEnum.Value1}; @}
        [Foreign(Language.CPlusPlus)]
        static MyByteEnum ReturnByteEnumValue2() @{ return @{MyByteEnum.Value2}; @}

        [Test]
        public void Tests()
        {
            Assert.AreEqual(MyEnum.Value1, UseEnum(MyEnum.Value1));
            Assert.AreEqual(MyEnum.Value2, UseEnum(MyEnum.Value2));
            Assert.AreEqual(MyByteEnum.Value1, UseByteEnum(MyByteEnum.Value1));
            Assert.AreEqual(MyByteEnum.Value2, UseByteEnum(MyByteEnum.Value2));
            Assert.AreEqual(MyEnum.Value1, ReturnEnumValue1());
            Assert.AreEqual(MyEnum.Value2, ReturnEnumValue2());
            Assert.AreEqual(MyByteEnum.Value1, ReturnByteEnumValue1());
            Assert.AreEqual(MyByteEnum.Value2, ReturnByteEnumValue2());
        }
    }
}
