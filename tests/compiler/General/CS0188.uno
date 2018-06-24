class Main
{
    struct S
    {
        public int a;

        void MyMethod() {}

        S(int i)
        {
            MyMethod(); // $E4511 [Ignore]
            a = 1;
        }

        S(float i)
        {
            a = 1;
            MyMethod();

        }

        S(double i) // $E4512 [Ignore]
        {

        }
    }
}
