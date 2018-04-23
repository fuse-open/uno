namespace Mono.test_415
{
    // Compiler options: -t:library
    
    using Uno;
    public abstract class MyTestAbstract
    {
        protected abstract string GetName();
        
        public MyTestAbstract()
        {
        }
    
        public void PrintName()
        {
            Console.WriteLine("Name=" + GetName());
        }
    }
}
