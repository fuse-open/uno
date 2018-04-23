using Uno;

namespace Lambdas
{
    class ReturnValues
    {
        void Use(int x) { }

        public void Run()
        {
            Action g = () =>
            {
                return 123; // $E2047 No implicit cast from 'int' to 'void'
            };

            int x = 0;

            g = () =>
                x = 123;

            Use(x);
            g();
            Func<int> h = () =>
            {
                return "hello"; // $E2047 No implicit cast from 'string' to 'int'
            };
            h = () =>
                "hello"; // $E2047 No implicit cast from 'string' to 'int'
            h();
        }
    }
}