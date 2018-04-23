namespace Uno.Time
{
    internal static class Converter
    {
        internal static int TicksToDays(long ticks)
        {
            if (ticks >= 0)
                return (int) ((ticks >> 14) / 52734375L);
            else
                return (int) ((ticks - (Constants.TicksPerStandardDay - 1)) /
                                Constants.TicksPerStandardDay);
        }
    }
}
