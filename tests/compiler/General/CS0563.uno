public class Main
{
    public static implicit operator int(Main x)
    {
        return 0;
    }

    public static implicit operator Main(int x)
    {
        return null;
    }

    public static int operator +(int aa, int bb) // $E4105
    {
        return 0;
    }
}