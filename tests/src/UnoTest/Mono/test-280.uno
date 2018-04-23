namespace Mono.test_280
{
    //
    // Thisis just a compilation test for bug 61593
    using Uno;
    namespace AppFramework.Util
    {
       public class Logic
       {
           static public bool EnumInSet(Enum anEnum, Enum[] checkSet)
           {
               foreach(Enum aVal in checkSet)
               {
                   if (aVal == anEnum)
                   {
                       return true;
                   }
               }
               return false;
           }
    
           [Uno.Testing.Test] public static void test_280() { Main(); }
        public static void Main() {}
       }
    }
}
