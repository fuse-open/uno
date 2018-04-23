namespace Mono.gtest_autoproperty_06
{
    class A { }
    class B { }
    
    interface I<T>
    {
        T Prop { get; set; }
    }
    
    class C : I<A>, I<B>
    {
        B I<B>.Prop { get; set; }
        A I<A>.Prop { get; set; }
    }
    
    class Program
    {
        [Uno.Testing.Test] public static void gtest_autoproperty_06() { Main(new string[0]); }
        public static void Main(string[] args)
        {
            C c = new C ();
        }
    }
}
