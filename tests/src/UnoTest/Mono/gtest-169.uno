namespace Mono.gtest_169
{
    class list <A> {
        public class Cons <T> : list <T> {   }
        public class Nil <T> : list <T> {  }
    }
    
    class C {
        public static void Rev<T> (list <T> y) {
            if (y is list<object>.Cons<T>)
                Console.WriteLine ("Cons");
            if (y is list<object>.Nil<T>)
                Console.WriteLine ("Nil");
        }
    }
    
    class M {
        [Uno.Testing.Test] public static void gtest_169() { Main(); }
        public static void Main() { 
            C.Rev (new list<object>.Cons <string> ());
            C.Rev (new list<object>.Nil <string> ());
         }
    }
}
