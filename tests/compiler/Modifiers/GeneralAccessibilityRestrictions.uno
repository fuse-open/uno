class Main {

    delegate int Delegate();

    private class B
    {
        private static int PrivateMethod()
        {
            return 0;
        }
    }

    public class A
    {
        public B Field = new B(); // $E4128

        public readonly B Const = new B(); // $E4128

        public B Method() // $E4128
        {
            return new B(); // $E [Ignore] The type B is less accessible than the method A.Method.
        }

        public B Prop { set {} } // $E4128

        Delegate d = (Delegate)B.PrivateMethod; // $E4128 $E4040

        public static B operator +(A m1, B m2) // $E4128 $E4128
        {
            return new B(); // $E [Ignore] The type B is less accessible than the operator A.operator +(A,B)
        }
    }
}
