namespace Mono.test_544
{
    enum ByteEnum : byte {
        One = 1,
        Two = 2
    }

    class X {
        [Uno.Testing.Test] public static void test_544() { Main(); }
        public static void Main()
        {
            ByteEnum b = ByteEnum.One;

            switch (b){
            case ByteEnum.One : return;
            case ByteEnum.One | ByteEnum.Two: return;
            }
        }
    }
}
