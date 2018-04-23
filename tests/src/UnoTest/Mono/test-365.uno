namespace Mono.test_365
{
    public enum LiteralType
        {
            Void,
        }
        
        class C
        {
            public LiteralType LiteralType                    
            {
                set
                {
                }
            }
            
            private LiteralType[] widenSbyte = new LiteralType[]
                {
                    LiteralType.Void
                };
                
            [Uno.Testing.Test] public static void test_365() { Main(); }
        public static void Main() {}
        }
}
