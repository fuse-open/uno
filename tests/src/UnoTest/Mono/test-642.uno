namespace Mono.test_642
{
    enum E : byte { }
    
    class C
    {
      static E e;
      static byte b;
      static ushort u;
    
      [Uno.Testing.Test] public static void test_642() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
      {
        b |= (byte) e;
        u |= (ushort) e;
        return b;
      }
    }
}
