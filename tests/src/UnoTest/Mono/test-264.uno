namespace Mono.test_264
{
    using Uno;
    
    public class Proef
    {
        private EventHandler _OnDoSomething = null;
    
        public event EventHandler OnDoSomething
        {
            add
            {
                _OnDoSomething += value;
            }
            remove
            {
                _OnDoSomething -= value;
            }
        }
    
        static void Temp(object s, EventArgs e)
        {
        }
    
        [Uno.Testing.Test] public static void test_264() { Main(); }
        public static void Main()
        {
            Proef temp = new Proef();
            temp.OnDoSomething += new EventHandler(Temp);
        }
    }
}
