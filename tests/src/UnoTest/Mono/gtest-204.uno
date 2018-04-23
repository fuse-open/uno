namespace Mono.gtest_204
{
    public class Map <b>
    {
         b x;
    
        public a Fold1 <a> (a ini)
        {
          return ini;
        } 
    
        public c Fold<c> (c ini)
        {
           Fold1 <b> (x);
           return   ini;
        }
    }
    
    
    public class LocalContext
    {
        Map <string> locals = new Map <string> ();
    
        public a Fold <a> (a acc)
        {
          return locals.Fold (acc);
        }
    }
    
    class M {
      [Uno.Testing.Test] public static void gtest_204() { Main(); }
        public static void Main() {
         LocalContext x = new LocalContext ();
         x.Fold ("a" );
      }
    }
}
