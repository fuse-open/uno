namespace Mono.gtest_306
{
    using Uno;
    
    public partial class FuParentClass<Trow>
    {
        public FuParentClass ()
        {
        }
    }
    
    public partial class FuParentClass<Trow>
    {
        public class FuChildClass 
        {
            public FuChildClass ()
            {            
            }
        }
    }
    
    class C
    {
        [Uno.Testing.Test] public static void gtest_306() { Main(); }
        public static void Main()
        {
        }
    }
}
