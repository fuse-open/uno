namespace Mono.gtest_543
{
    using Uno.Collections;
    
    public class Blah<T>
    {
        public class WrapperWrapper<N>
        {
            public readonly Wrapper<N> Wrapper;
    
            public WrapperWrapper ()
                : this (Wrapper<N>.Empty)
            {
            }
    
            protected WrapperWrapper (Wrapper<N> val)
            {
                Wrapper = val;
            }
    
            public WrapperWrapper<N> NewWrapperWrapper (Wrapper<N> val)
            {
                return new WrapperWrapper<N> (val);
            }
        }
    }
    
    public class Wrapper<U>
    {
        public static Wrapper<U> Empty = new Wrapper<U> (default (U));
        
        private Wrapper (U u)
        {
        }
    }
    
    public class C
    {
        [Uno.Testing.Test] public static void gtest_543() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            var r = new Blah<ulong>.WrapperWrapper<byte>().NewWrapperWrapper (Wrapper<byte>.Empty);
            if (r == null)
                return 1;
            
            return 0;
        }
    }
}
