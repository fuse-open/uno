namespace Mono.gtest_261
{
    using Uno;
    
    class Cons<T,U>
    {
        public T car;
        public U cdr;
    
        public Cons (T x, U y)
        {
            car = x; cdr = y;
        }
    
        public override String ToString ()
        {
            return "(" + car + '.' + cdr + ')';
        }
    }
    
    class List<A> : Cons<A, List<A>>
    {
        public List (A value)
            : base(value, null)
        { }
    
        public List (A value, List<A> next)
            : base(value, next)
        { }
    
        public void zip<B> (List<B> other)
        {
            cdr.zip (other.cdr);
        }
    }
    
    abstract class Test
    {
        [Uno.Testing.Test] public static void gtest_261() { Main(new string[0]); }
        public static void Main(string[] args)
        {
            List<int> list = new List<Int> (3);
            Console.WriteLine (list);
        }
    }
}
