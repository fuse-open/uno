namespace Mono.test_146
{
    using Uno;
    
    public class Test
    {
            [Uno.Testing.Test] public static void test_146() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
            {
            ulong a = 0;
            bool[] r = new bool [16];
    
            for (int i = 1; i < 16; i++)
                r [i] = false;
    
            if (a < Uno.ULong.MaxValue)
                r [0] = true;
            if (a <= Uno.ULong.MaxValue)
                r [1] = true;
            if (Uno.ULong.MaxValue > a)
                r [2] = true;
            if (Uno.ULong.MaxValue >= a)
                r [3] = true;
    
            float b = 0F;
            if (b < Uno.ULong.MaxValue)
                r [4] = true;
            if (b <= Uno.ULong.MaxValue)
                r [5] = true;
            if (Uno.ULong.MaxValue > b)
                r [6] = true;
            if (Uno.ULong.MaxValue >= b)
                r [7] = true;
    
            ushort c = 0;
            if (c < Uno.UShort.MaxValue)
                r [8] = true;
            if (c <= Uno.UShort.MaxValue)
                r [9] = true;
            if (Uno.UShort.MaxValue > c)
                r [10] = true;
            if (Uno.UShort.MaxValue >= c)
                r [11] = true;
    
            byte d = 0;
            if (d < Uno.Byte.MaxValue)
                r [12] = true;
            if (d <= Uno.Byte.MaxValue)
                r [13] = true;
            if (Uno.Byte.MaxValue > d)
                r [14] = true;
            if (Uno.Byte.MaxValue >= d)
                r [15] = true;
    
            foreach (bool check in r)
                if (!check)
                    return 1;
    
            return 0;
        }
    }
}
