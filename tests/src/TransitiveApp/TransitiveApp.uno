using Uno;

namespace TransitiveApp
{
    public class TransitiveApp : Application
    {
        public TransitiveApp()
        {
            // App refences A, which has a transitive reference to B.
            // This means that App gets an implicit reference to B through A.
            Transitive.B.Foo();
        }
    }
}
