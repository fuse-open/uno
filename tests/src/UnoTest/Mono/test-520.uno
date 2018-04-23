namespace Mono.test_520
{
    using Uno;
    
    class FakeInt {
        private long _value;
        public FakeInt (long val) { _value = val; }
        public static implicit operator long (FakeInt self) { return self._value; }
    }
    
    class MainClass {
        [Uno.Testing.Test] public static void test_520() { Main(); }
        public static void Main()
        {
            if(new FakeInt (42) != 42)
                throw new Exception ();
        }
    }
}
