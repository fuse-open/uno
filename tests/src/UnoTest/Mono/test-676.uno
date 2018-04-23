namespace Mono.test_676
{
    using Uno;
    
    namespace N
    {
        class Item
        {
            public Item ()
            {
            }
            
            public enum ItemSlot
            {
                ItemM1,
                ItemM2
            }
        }
    }
    
    namespace N
    {
        public class Test
        {
            Item this [Test slot]
            {
                get { return null; }
            }
            
            void Foo (Item.ItemSlot i)
            {
                object oo = this [null];
                
                switch (i)
                {
                    case Item.ItemSlot.ItemM1:
                        break;
                }
            }
        
            [Uno.Testing.Test] public static void test_676() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
            {
                return 0;
            }
        }
    }
}
