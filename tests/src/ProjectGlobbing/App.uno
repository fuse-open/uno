using Uno;
using Projects;

namespace ProjectGlobbing
{
    public class App : Application
    {
        public App()
        {
            A.Foo();
            B.Foo();
        }
    }
}
