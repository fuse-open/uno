using Uno;

namespace Lambdas
{
    class DefaultParameters
    {
        public void Run()
        {
            Action<int> f = (int x = 123) => x = 223; // $E0000
            f(41);
        }
    }
}