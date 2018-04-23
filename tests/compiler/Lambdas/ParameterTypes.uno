using Uno;

namespace Lambdas
{
    class ParameterTypes
    {
        public void Run()
        {
            Action<int> f = (int x) => { };
            f = (x) => { };
            f(41);

            Action<int, float> g = (int x, y) => { }; // $E0748 Inconsistent lambda parameter usage; all parameter types must either be explicit or implicit

            g = (int x) => { }; // $E1593 Delegate 'Uno.Action<int, float>' does not take 1 arguments

            g = (float x, int y) => { }; // $E1661 Cannot convert anonymous method block to delegate type 'Uno.Action<int, float>' because the specified block's parameter types do not match the delegate parameter types
            g(41, 1.0f);
        }
    }
}