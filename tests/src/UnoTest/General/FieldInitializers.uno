using Uno;
using Uno.Testing;

namespace UnoTest.General
{
    public class FieldInitializers
    {
        static int StaticField1, StaticField2 = 3;
        static string StaticStr1, StaticStr2 = "hei", StaticStr3;

        int Field1, Field2 = 4;
        string Str1, Str2 = "hei", Str3 = "heisann";

        public FieldInitializers()
        {
            StaticStr3 = "halla";
        }

        [Test]
        public void Run()
        {
            Assert.IsFalse(StaticField1 != 0 ||
                StaticField2 != 3 ||
                StaticStr1 != null ||
                StaticStr2 != "hei" ||
                StaticStr3 != "halla");
        }
    }

    public struct FieldInitializersStruct
    {
        static int StaticField1, StaticField2 = 3;
        static string StaticStr1, StaticStr2 = "hei", StaticStr3;

        int Field1, Field2 = 0;
        string Str1, Str2 = default(string), Str3 = null;

        [Test]
        public static void Run()
        {
            Assert.IsFalse(StaticField1 != 0 ||
                StaticField2 != 3 ||
                StaticStr1 != null ||
                StaticStr2 != "hei");
        }
    }
}
