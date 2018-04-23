using Uno;

namespace Lambdas
{
    class ReturnValues
    {
        int ReturnInt() { return 41; }

        void Use(int x) { }

        public void Run()
        {
            Func<int> f = () =>
            {
                return; // $E0000 Non-void lambda must return a value
            };
            f();

            int x = 0;

            Action g = () =>
                x = ReturnInt();
            g();

            Use(x);
        }
    }
}